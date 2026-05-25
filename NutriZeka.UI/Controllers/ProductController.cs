using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NutriZeka.WebUI.Dtos.ProductDtos;
using System.Text;
using System.Net.Http.Headers;

namespace NutriZeka.WebUI.Controllers
{
    public class ProductController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiUrl = "http://[Ipv4 adresin]:7009/api/Products";

        public ProductController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // 1. Ürün Listeleme & Filtreleme
        public async Task<IActionResult> Index(string searchTerm, string category)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiUrl}?searchTerm={searchTerm}&category={category}");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var token = JToken.Parse(jsonData);
                List<ResultProductDto> values;

                if (token is JObject obj && obj["data"] != null)
                    values = obj["data"].ToObject<List<ResultProductDto>>();
                else
                    values = JsonConvert.DeserializeObject<List<ResultProductDto>>(jsonData);

                // İstatistikler ve Kategoriler
                ViewBag.TotalCount = values.Count;
                var distinctCategories = values
                    .Where(x => !string.IsNullOrEmpty(x.CategoriesTr))
                    .Select(x => x.CategoriesTr.Split(',')[0].Trim())
                    .Distinct()
                    .OrderBy(x => x).ToList();

                ViewBag.Categories = distinctCategories;
                ViewBag.CategoryCount = distinctCategories.Count;
                ViewBag.SearchTerm = searchTerm;
                ViewBag.SelectedCategory = category;

                return View(values);
            }
            return View(new List<ResultProductDto>());
        }

        // 2. CSV Import (MODAL İÇİN)
        [HttpPost]
        public async Task<IActionResult> ImportCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["ErrorMessage"] = "Lütfen geçerli bir dosya seçin.";
                return RedirectToAction("Index");
            }

            var client = _httpClientFactory.CreateClient();
            using var content = new MultipartFormDataContent();

            // Dosyayı byte dizisine çevirerek gönderiyoruz (En güvenli yol)
            var fileStream = file.OpenReadStream();
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("text/csv");

            // Backend'in beklediği parametre adının "file" olduğundan emin olduk
            content.Add(fileContent, "file", file.FileName);

            var response = await client.PostAsync($"{_apiUrl}/import", content);

            if (response.IsSuccessStatusCode)
                TempData["SuccessMessage"] = "Ürünler toplu olarak başarıyla yüklendi!";
            else
                TempData["ErrorMessage"] = "Yükleme başarısız. CSV formatını kontrol edin.";

            return RedirectToAction("Index");
        }

        // Parametre ismini kafa karışıklığı olmasın diye 'barcode' yaptık
        public async Task<IActionResult> DeleteProduct(string barcode)
        {
            var client = _httpClientFactory.CreateClient();

            // SWAGGER'DAKİ DOĞRU YOL: api/Products/barcode/{barcode}
            var response = await client.DeleteAsync($"{_apiUrl}/barcode/{barcode}");

            if (response.IsSuccessStatusCode)
                TempData["SuccessMessage"] = "Ürün başarıyla silindi.";
            else
                TempData["ErrorMessage"] = "Silme başarısız. Barkodu kontrol edin.";

            return RedirectToAction("Index");
        }

        // 4. Yeni Ürün Ekleme
        [HttpGet]
        public IActionResult CreateProduct() => View();

        [HttpPost]
        public async Task<IActionResult> CreateProduct(CreateProductDto createProductDto)
        {
            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonConvert.SerializeObject(createProductDto);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(_apiUrl, content);
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Yeni ürün eklendi!";
                return RedirectToAction("Index");
            }
            return View(createProductDto);
        }

        // 5. Güncelleme
        [HttpGet]
        public async Task<IActionResult> UpdateProduct(string barcode)
        {
            if (string.IsNullOrEmpty(barcode))
            {
                TempData["ErrorMessage"] = "Düzenlenecek ürünün barkod bilgisi eksik!";
                return RedirectToAction("Index");
            }

            var client = _httpClientFactory.CreateClient();
            // API rotası Swagger'daki gibi barkod üzerinden gidiyor: /api/Products/barcode/{barcode}
            var response = await client.GetAsync($"{_apiUrl}/barcode/{barcode}");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var token = JToken.Parse(jsonData);
                UpdateProductDto value;

                if (token is JObject obj && obj["data"] != null)
                    value = obj["data"].ToObject<UpdateProductDto>();
                else
                    value = JsonConvert.DeserializeObject<UpdateProductDto>(jsonData);

                return View(value);
            }

            TempData["ErrorMessage"] = "Ürün bilgileri API'den çekilemedi.";
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateProduct(UpdateProductDto updateProductDto)
        {
            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonConvert.SerializeObject(updateProductDto);
            StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            // SWAGGER: PUT /api/Products/barcode/{barcode}
            // Rota artık barkod üzerinden gidiyor!
            var response = await client.PutAsync($"{_apiUrl}/barcode/{updateProductDto.Barcode}", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Ürün başarıyla güncellendi!";
                return RedirectToAction("Index");
            }

            // Hata durumunda mesajı göster ve sayfada kal
            var errorMsg = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", $"Güncelleme başarısız: {errorMsg}");

            return View(updateProductDto);
        }
    }
}