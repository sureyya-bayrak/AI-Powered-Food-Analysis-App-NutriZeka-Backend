using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NutriZeka.UI.Dtos.HistoryDtos;
using System.Text;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace NutriZeka.WebUI.Controllers
{
    [Authorize]
    public class HistoryController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        // Swagger'daki çoğul isimlendirme: ScanHistories
        private readonly string _apiBaseUrl = "http://[Ipv4 adresin]:7009/api/ScanHistories";

        public HistoryController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient GetAuthenticatedClient()
        {
            var client = _httpClientFactory.CreateClient();
            var token = HttpContext.Request.Cookies["NutriZekaApiToken"];

            if (!string.IsNullOrEmpty(token))
            {
                // --- KRİTİK JWT TEMİZLİĞİ (UserController ile aynı) ---
                token = token.Replace("\"", "").Trim();
                token = Regex.Replace(token, @"[^\x00-\x7F]+", "");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            return client;
        }

        public async Task<IActionResult> Index()
        {
            var client = GetAuthenticatedClient();
            var response = await client.GetAsync(_apiBaseUrl);

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var tokenResult = JToken.Parse(jsonData);
                List<ScanHistoryDto> values = null;

                // API'den gelen "data" zarfını açıyoruz
                if (tokenResult is JObject && tokenResult["data"] != null)
                {
                    values = tokenResult["data"].ToObject<List<ScanHistoryDto>>();
                }
                else
                {
                    values = JsonConvert.DeserializeObject<List<ScanHistoryDto>>(jsonData);
                }

                return View(values ?? new List<ScanHistoryDto>());
            }

            // Eğer Yetki hatası veya başka bir hata varsa boş liste dön
            return View(new List<ScanHistoryDto>());
        }

        public async Task<IActionResult> UserHistory(Guid id)
        {
            var client = GetAuthenticatedClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/user/{id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var tokenResult = JToken.Parse(jsonData);
                List<ScanHistoryDto> values = null;

                if (tokenResult is JObject && tokenResult["data"] != null)
                    values = tokenResult["data"].ToObject<List<ScanHistoryDto>>();
                else
                    values = JsonConvert.DeserializeObject<List<ScanHistoryDto>>(jsonData);

                ViewBag.SelectedUserId = id;
                return View("Index", values ?? new List<ScanHistoryDto>());
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteHistory(Guid userId)
        {
            var client = GetAuthenticatedClient();
            // Swagger'daki delete yoluna dikkat: clear-all/{userId}
            var response = await client.DeleteAsync($"{_apiBaseUrl}/clear-all/{userId}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return BadRequest("Silme işlemi başarısız.");
        }
    }
}