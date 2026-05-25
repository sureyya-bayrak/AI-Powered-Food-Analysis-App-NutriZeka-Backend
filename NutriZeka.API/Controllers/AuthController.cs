using Microsoft.AspNetCore.Mvc;
using NutriZeka.Application.DTOs;
using NutriZeka.Application.Interfaces;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Google.Apis.Auth;
namespace NutriZeka.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public AuthController(IUserService userService, ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto dto)
        {
            try
            {
                var userProfile = await _userService.RegisterAsync(dto);
                var token = _tokenService.CreateToken(userProfile);

                return Ok(new { Token = token, Profile = userProfile });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto dto)
        {
            var userProfile = await _userService.ValidateUserAsync(dto);

            if (userProfile == null)
                return Unauthorized(new { message = "E-posta veya şifre hatalı!" });

            var token = _tokenService.CreateToken(userProfile);

            return Ok(new { Token = token, Profile = userProfile });

        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto request)
        {
            try
            {
                // 1. Google'dan gelen şifreyi doğrula
                var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken);

                // 2. Senin IUserService'in üzerinden kullanıcıyı bul (veya yoksa kaydet)
                var user = await _userService.HandleGoogleUserAsync(payload.Email, payload.Name, payload.Picture);

                // 3. Senin ITokenService'in ile kendi sisteminin Token'ını üret
                var token = _tokenService.CreateToken(user);

                // 4. Flutter'a Token'ı ve kullanıcı bilgilerini gönder
                return Ok(new { Token = token, Profile = user });
            }
            catch (InvalidJwtException)
            {
                return Unauthorized(new { message = "Geçersiz Google Token'ı." });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}