using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using NutriZeka.UI.Dtos.UserDtos;
using Newtonsoft.Json.Linq;

namespace NutriZeka.WebUI.Controllers
{
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiAuthUrl = "http://[Ipv4 adresin]:7009/api/Auth";

        public AuthController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(UserLoginDto dto)
        {
            // === 1. GÜVENLİK DUVARI: SADECE ADMİN MAİLİNE İZİN VER ===
            if (dto.Email != "admin@nutrizeka.com")
            {
                ViewBag.ErrorMessage = "Bu panele sadece sistem yöneticisi giriş yapabilir.";
                return View(dto);
            }
            // =========================================================

            var client = _httpClientFactory.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{_apiAuthUrl}/login", content);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                string token = "";

                // API'den gelen veriyi güvenle çıkarıyoruz
                if (responseData.Trim().StartsWith("{"))
                {
                    var jObject = JObject.Parse(responseData);
                    token = jObject["token"]?.ToString() ?? jObject["data"]?["token"]?.ToString();
                }
                else
                {
                    token = responseData;
                }

                token = token?.Replace("\"", "").Trim();

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, dto.Email),
                    new Claim(ClaimTypes.Role, "Admin")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties { IsPersistent = true };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                Response.Cookies.Append("NutriZekaApiToken", token, new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTime.Now.AddDays(7)
                });

                return RedirectToAction("Index", "Home");
            }

            ViewBag.ErrorMessage = "Giriş yapılamadı. Lütfen şifrenizi kontrol edin.";
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Response.Cookies.Delete("NutriZekaApiToken");
            Response.Cookies.Delete("NutriZekaToken");

            return RedirectToAction("Login", "Auth");
        }
    }
}