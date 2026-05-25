using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NutriZeka.Application.DTOs;
using NutriZeka.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NutriZeka.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;

        public TokenService(IConfiguration config)
        {
            _config = config;
            
            // appsettings.json dosyasından gizli anahtarı alıyoruz. 
            var jwtKey = _config["JwtSettings:Key"];
            
            // Eğer şifre yoksa veya "vitrin" dosyasındaki sahte metin kalmışsa sistemi durduruyoruz
            if (string.IsNullOrEmpty(jwtKey) || jwtKey == "BURASI_GIZLIDIR_GERCEK_SIFRE_YAZILMAMALIDIR")
            {
                throw new InvalidOperationException("JWT Secret Key eksik veya varsayılan değerde bırakılmış!");
            }

            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        }

        public string CreateToken(UserProfileDto user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("FullName", user.FullName ?? "")
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7), // Token 7 gün geçerli
                SigningCredentials = creds,
                Issuer = _config["JwtSettings:Issuer"] ?? "NutriZekaAPI",
                Audience = _config["JwtSettings:Audience"] ?? "NutriZekaApp"
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}