using System;
using System.Threading.Tasks;
using NutriZeka.Application.DTOs;
using NutriZeka.Domain.Entities;

namespace NutriZeka.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserProfileDto> RegisterAsync(UserRegisterDto dto);
        Task<UserProfileDto> GetProfileAsync(Guid id);
        Task<UserProfileDto> UpdateProfileAsync(Guid id, UserUpdateDto dto);
        Task UpdatePreferencesAsync(Guid id, UserPreferencesUpdateDto dto);
        Task DeleteUserAsync(Guid id);
        // "User" yazan yeri "UserProfileDto" olarak değiştirdik

        Task DeleteProfileImageAsync(Guid userId);
        Task<UserProfileDto> HandleGoogleUserAsync(string email, string name, string pictureUrl);
        Task<IEnumerable<UserProfileDto>> GetAllUsersAsync();
        Task<string> UploadProfileImageAsync(Guid userId, Stream fileStream, string fileName);
        // UserService'deki isimle aynı olmalı
        // Not: Login işleminde JWT Token üreteceğimiz için onu bir sonraki adımda (AuthService veya Controller içinde) ele alacağız.
        // Şimdilik sadece şifre doğrulaması yapan bir metot ekleyelim:
        Task<UserProfileDto> ValidateUserAsync(UserLoginDto dto);
    }
}