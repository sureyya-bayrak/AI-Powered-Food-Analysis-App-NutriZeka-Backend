using NutriZeka.Application.Interfaces.Services;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NutriZeka.Infrastructure.Services
{
    public class GeminiService : IGenerativeAiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "[Gemini API Key buraya gelecek]";

        public GeminiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GenerateResponseAsync(string prompt)
        {
            try
            {
                var requestBody = new
                {
                    contents = new[] { new { parts = new[] { new { text = prompt } } } }
                };

                var jsonRequest = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                string url = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key=" + _apiKey;

                Console.WriteLine(">>> GIDEN URL: " + url);

                var response = await _httpClient.PostAsync(url, content);
                var responseBody = await response.Content.ReadAsStringAsync();

                Console.WriteLine(">>> STATUS: " + response.StatusCode);
                Console.WriteLine(">>> RESPONSE: " + responseBody);

                if (response.IsSuccessStatusCode)
                {
                    using var doc = JsonDocument.Parse(responseBody);
                    var answer = doc.RootElement
                        .GetProperty("candidates")[0]
                        .GetProperty("content")
                        .GetProperty("parts")[0]
                        .GetProperty("text")
                        .GetString();

                    return answer?.Trim() ?? "Analiz sonucu boş döndü.";
                }
                else
                {
                    return $"Gemini API Hatası ({response.StatusCode}): {responseBody}";
                }
            }
            catch (System.Exception ex)
            {
                return $"Bağlantı Hatası: {ex.Message}";
            }
        }
    }
}