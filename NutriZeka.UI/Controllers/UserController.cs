using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;
using NutriZeka.UI.Dtos.UserDtos;
using Microsoft.AspNetCore.Authorization; // [Authorize] için gerekli
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace NutriZeka.WebUI.Controllers
{
    [Authorize] // <--- BU SATIR EKSİKTİ, GERİ EKLEDİK (KRİTİK!)
    public class UserController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IWebHostEnvironment _env;
        private readonly string _apiBaseUrl = "http://[Ipv4 adresin]:7009/api/Users";

        public UserController(IHttpClientFactory httpClientFactory, IWebHostEnvironment env)
        {
            _httpClientFactory = httpClientFactory;
            _env = env;
        }

        private HttpClient GetAuthenticatedClient()
        {
            var client = _httpClientFactory.CreateClient();
            var token = HttpContext.Request.Cookies["NutriZekaApiToken"];

            if (!string.IsNullOrEmpty(token))
            {
                // Token temizliği (ASCII ve Tırnak hataları için)
                token = token.Replace("\"", "").Trim();
                token = Regex.Replace(token, @"[^\x00-\x7F]+", "");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            return client;
        }

        public async Task<IActionResult> Index()
        {
            var client = GetAuthenticatedClient();
            var responseMessage = await client.GetAsync(_apiBaseUrl);

            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();

                // JToken ile her türlü veri yapısını (data kutusu olsa da olmasa da) karşılıyoruz
                var tokenResult = JToken.Parse(jsonData);
                List<ResultUserDto> values;

                if (tokenResult is JObject && tokenResult["data"] != null)
                {
                    // Veri { "data": [...] } şeklindeyse
                    values = tokenResult["data"].ToObject<List<ResultUserDto>>();
                }
                else
                {
                    // Veri direkt [...] şeklindeyse
                    values = JsonConvert.DeserializeObject<List<ResultUserDto>>(jsonData);
                }

                return View(values ?? new List<ResultUserDto>());
            }

            // Hata durumunda boş liste dön (Yetki hatası varsa buraya düşer)
            return View(new List<ResultUserDto>());
        }

        [HttpGet]
        public IActionResult CreateUser() => View();

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var client = GetAuthenticatedClient();
            var jsonData = JsonConvert.SerializeObject(dto);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            // API'ye kayıt isteği gönderiliyor
            var responseMessage = await client.PostAsync(_apiBaseUrl + "/register", content);

            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            // --- BURASI EKLEDİĞİMİZ KRİTİK KISIM ---
            // API'den gelen ham hata mesajını okuyoruz
            var errorBody = await responseMessage.Content.ReadAsStringAsync();

            // Eğer API "Bu e-posta zaten var" veya "Şifre kurala uymuyor" diyorsa bunu yakalayalım
            try
            {
                var jObj = JObject.Parse(errorBody);
                // API genellikle hatayı 'message' veya 'errors' içinde döner
                ViewBag.ErrorMessage = jObj["message"]?.ToString() ?? jObj["errors"]?.ToString() ?? errorBody;
            }
            catch
            {
                // Eğer gelen yanıt JSON değilse direkt metni basalım
                ViewBag.ErrorMessage = "API Hatası: " + errorBody;
            }

            return View(dto);
        }
        [HttpGet]
        public async Task<IActionResult> UpdateUser(Guid id)
        {
            var client = GetAuthenticatedClient();
            // Swagger'a göre: GET /api/Users/{id}
            var responseMessage = await client.GetAsync($"{_apiBaseUrl}/{id}");

            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var tokenResult = JToken.Parse(jsonData);

                GetByIdUserDto value = null;

                // API genelde veriyi bir "data" zarfı içinde gönderir
                if (tokenResult is JObject && tokenResult["data"] != null)
                    value = tokenResult["data"].ToObject<GetByIdUserDto>();
                else
                    value = JsonConvert.DeserializeObject<GetByIdUserDto>(jsonData);

                if (value != null)
                {
                    var updateModel = new UpdateUserDto
                    {
                        Id = value.Id,
                        FullName = value.FullName,
                        PhoneNumber = value.PhoneNumber,
                        ProfileImageUrl = value.ProfileImageUrl,
                        IsGlutenFree = value.IsGlutenFree,
                        IsLactoseFree = value.IsLactoseFree,
                        IsPalmOilFree = value.IsPalmOilFree
                    };
                    return View(updateModel);
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser(UpdateUserDto dto)
        {
            var client = GetAuthenticatedClient();

            // 1. Profil Resmi Yükleme İşlemi (Aynı kaldı)
            if (dto.ProfileImageFile != null && dto.ProfileImageFile.Length > 0)
            {
                var extension = Path.GetExtension(dto.ProfileImageFile.FileName).ToLower();
                var newImageName = $"user_{dto.Id}_{DateTime.UtcNow.Ticks}{extension}";
                var path = Path.Combine(_env.WebRootPath, "uploads", newImageName);

                if (!Directory.Exists(Path.Combine(_env.WebRootPath, "uploads")))
                    Directory.CreateDirectory(Path.Combine(_env.WebRootPath, "uploads"));

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await dto.ProfileImageFile.CopyToAsync(stream);
                }
                dto.ProfileImageUrl = $"/uploads/{newImageName}";
            }

            // 2. Temel Bilgileri Güncelleme (PUT /api/Users/{id})
            var updateData = new { dto.FullName, dto.PhoneNumber, dto.ProfileImageUrl };
            var updateContent = new StringContent(JsonConvert.SerializeObject(updateData), Encoding.UTF8, "application/json");
            await client.PutAsync($"{_apiBaseUrl}/{dto.Id}", updateContent);

            // 3. Diyet Tercihlerini Güncelleme (Swagger'a göre: PATCH /api/Users/{id}/preferences)
            var prefData = new { dto.IsGlutenFree, dto.IsLactoseFree, dto.IsPalmOilFree };
            var prefContent = new StringContent(JsonConvert.SerializeObject(prefData), Encoding.UTF8, "application/json");

            // BURASI DEĞİŞTİ: PutAsync yerine PatchAsync (veya SendAsync) kullanmalısın
            var patchRequest = new HttpRequestMessage(new HttpMethod("PATCH"), $"{_apiBaseUrl}/{dto.Id}/preferences")
            {
                Content = prefContent
            };
            await client.SendAsync(patchRequest);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var client = GetAuthenticatedClient();
            await client.DeleteAsync($"{_apiBaseUrl}/{id}");
            return RedirectToAction("Index");
        }
    }
}