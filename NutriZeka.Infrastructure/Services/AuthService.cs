using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NutriZeka.Application.Interfaces;
using NutriZeka.Domain.Entities;
using NutriZeka.Domain.Interfaces;

namespace NutriZeka.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<string> GoogleLoginAsync(string idToken)
        {
            // 1. Google'dan gelen bileti (token) doğrula
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken);

            // 2. Kullanıcıyı e-posta adresiyle veritabanında ara
            var existingUser = await _userRepository.GetByEmailAsync(payload.Email);

            // --- İŞTE GOOGLE İLE KAYIT (REGISTER) KISMI BURASI ---
            if (existingUser == null)
            {
                existingUser = new User
                {
                    Id = Guid.NewGuid(),
                    Email = payload.Email,
                    FullName = payload.Name,
                    ProfileImageUrl = payload.Picture,
                    IsGoogleUser = true, // Senin tablona göre Google kullanıcısı olduğunu işaretliyoruz
                    PasswordHash = "GOOGLE_AUTH", // Google girişinde şifre olmaz, buraya dummy bir veri veya null girebiliriz.
                    ProfileCompletionRate = 60
                };

                await _userRepository.AddAsync(existingUser);
                await _userRepository.SaveChangesAsync();
            }
            // --------------------------------------------------------

            // --- İŞTE GOOGLE İLE GİRİŞ (LOGIN) KISMI BURASI ---
            // Kullanıcı ister yeni kaydedilmiş olsun, ister zaten veritabanında var olsun...
            // Ona uygulamamızın kapılarını açacak olan JWT Token'ı üretip veriyoruz.
            return GenerateJwtToken(existingUser);
        }
        public async Task<string> LoginAsync(string email, string password)
        {
            // 1. ADIM: Sadece senin e-postanın admin paneline girmesine izin veriyoruz.
            if (email != "admin@nutrizeka.com")
            {
                throw new UnauthorizedAccessException("Bu panele sadece sistem yöneticisi erişebilir.");
            }

            var user = await _userRepository.GetByEmailAsync(email);

            // 2. ADIM: Kullanıcı var mı ve senin Google hesabın mı kontrolü
            if (user == null)
            {
                throw new UnauthorizedAccessException("Yönetici kaydı bulunamadı.");
            }

            // 3. ADIM: Şifre Kontrolü
            // Google ile kayıt olduğunda PasswordHash = "GOOGLE_AUTH" olarak setlenmiş.
            // Eğer veritabanında şifren hala "GOOGLE_AUTH" ise, giriş yapabilmek için
            // şifre kısmına da "GOOGLE_AUTH" yazman gerekir (veya DB'den şifreni güncellemelisin).

            if (user.PasswordHash != password)
            {
                throw new UnauthorizedAccessException("E-posta veya şifre hatalı.");
            }

            return GenerateJwtToken(user);
        }

        // Token Üreten Yardımcı Metot
        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName ?? string.Empty),
                new Claim("IsGoogleUser", user.IsGoogleUser.ToString()) // Token içine Google kullanıcısı olduğunu da gömebiliriz
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}