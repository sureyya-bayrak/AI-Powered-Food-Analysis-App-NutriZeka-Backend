using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriZeka.Application.DTOs;
using NutriZeka.Application.Interfaces.Services;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NutriZeka.API.Controllers
{
    [Authorize] // Sadece giriş yapmış kullanıcılar NutriZeka AI'ya soru sorabilir
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IAIAnalysisService _aiAnalysisService;

        public ChatController(IAIAnalysisService aiAnalysisService)
        {
            _aiAnalysisService = aiAnalysisService;
        }

        /// <summary>
        /// Kullanıcının seçtiği soru tipine göre (Sağlık profili, içerik analizi veya alternatif) 
        /// Gemini AI üzerinden veya Cache'den cevap döner.
        /// </summary>
        [HttpPost("ask")]
        public async Task<IActionResult> AskQuestion([FromBody] AIAnalysisRequestDto request)
        {
            try
            {
                // 🚀 JWT Token içinden giriş yapan kullanıcının ID'sini çekiyoruz
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid userId))
                {
                    return Unauthorized(new { message = "Geçersiz kullanıcı oturumu." });
                }

                // 🚀 İş mantığı katmanına gidiyoruz (Cache kontrolü + Gemini Analizi burada gerçekleşir)
                var aiResponse = await _aiAnalysisService.GetAnalysisAsync(userId, request);

                return Ok(new { answer = aiResponse });
            }
            catch (Exception ex)
            {
                // Hata durumunda kullanıcıya dostça bir mesaj dönüyoruz
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}