using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriZeka.Application.DTOs;
using NutriZeka.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace NutriZeka.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")] // Burası artık otomatik olarak "api/users" olacak
    [ApiController]
    public class UsersController : ControllerBase // "s" takısını ekledik
    {
        private readonly IUserService _userService;

        // Kurucu metodun (Constructor) ismini de sınıf adıyla aynı yapıyoruz
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // ... Diğer tüm Get, Put, Delete metotları aynı kalıyor ...

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProfile(Guid id)
        {
            try { return Ok(await _userService.GetProfileAsync(id)); }
            catch (Exception ex) { return NotFound(new { message = ex.Message }); }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProfile(Guid id, UserUpdateDto dto)
        {
            try { return Ok(await _userService.UpdateProfileAsync(id, dto)); }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpPatch("{id}/preferences")]
        public async Task<IActionResult> UpdatePreferences(Guid id, UserPreferencesUpdateDto dto)
        {
            try
            {
                await _userService.UpdatePreferencesAsync(id, dto);
                return Ok(new { message = "Tercihler başarıyla güncellendi." });
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            try
            {
                await _userService.DeleteUserAsync(id);
                return Ok(new { message = "Hesap başarıyla silindi." });
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // Hata buradaydı; Interface'deki yeni isme göre güncelliyoruz
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }
        [HttpPost("{id}/upload-image")]
        public async Task<IActionResult> UploadProfileImage(Guid id, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new { message = "Lütfen geçerli bir resim dosyası seçin." });

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var extension = Path.GetExtension(file.FileName).ToLower();
                if (!Array.Exists(allowedExtensions, e => e == extension))
                    return BadRequest(new { message = "Sadece .jpg, .jpeg ve .png formatları desteklenmektedir." });

                // ŞOV BURADA: IFormFile paketini açıp, içindeki saf Stream'i servise yolluyoruz
                using var stream = file.OpenReadStream();
                var imageUrl = await _userService.UploadProfileImageAsync(id, stream, file.FileName);

                return Ok(new { profileImageUrl = imageUrl, message = "Profil resmi başarıyla yüklendi." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }


        }

        // YENİ ENDPOINT: Profil fotoğrafını kaldırır
        [HttpDelete("{id}/delete-image")]
        public async Task<IActionResult> DeleteProfileImage(Guid id)
        {
            try
            {
                await _userService.DeleteProfileImageAsync(id);
                return Ok(new { message = "Profil fotoğrafınız başarıyla kaldırıldı." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}