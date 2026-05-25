using AutoMapper;
using NutriZeka.Application.DTOs;
using NutriZeka.Application.Interfaces;
using NutriZeka.Domain.Entities;
using NutriZeka.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace NutriZeka.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserProfileDto> RegisterAsync(UserRegisterDto dto)
        {
            if (await _userRepository.ExistsByEmailAsync(dto.Email))
                throw new Exception("Bu e-posta adresi zaten kullanımda.");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                CreatedAt = DateTime.UtcNow,
                FullName = dto.FullName,
                ProfileCompletionRate = 40,
                IsGoogleUser = false,
                IsGlutenFree = false,
                IsLactoseFree = false,
                IsPalmOilFree = false,
                // AI Limitleri için başlangıç değerleri
                LastAiRequestDate = DateTime.UtcNow,
                DailyAiRequestCount = 0
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return _mapper.Map<UserProfileDto>(user);
        }

        public async Task<UserProfileDto> ValidateUserAsync(UserLoginDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return null;

            return _mapper.Map<UserProfileDto>(user);
        }

        public async Task<UserProfileDto> GetProfileAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) throw new Exception("Kullanıcı bulunamadı.");

            return _mapper.Map<UserProfileDto>(user);
        }

        public async Task<UserProfileDto> UpdateProfileAsync(Guid id, UserUpdateDto dto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) throw new Exception("Kullanıcı bulunamadı.");

            user.FullName = dto.FullName ?? user.FullName;
            user.PhoneNumber = dto.PhoneNumber ?? user.PhoneNumber;
            user.ProfileImageUrl = dto.ProfileImageUrl ?? user.ProfileImageUrl;

            // --- OYUNLAŞTIRMA (Hesaplama Mantığın Korundu) ---
            int rate = 20;
            if (!string.IsNullOrEmpty(user.FullName)) rate += 20;
            if (!string.IsNullOrEmpty(user.PhoneNumber)) rate += 20;
            if (!string.IsNullOrEmpty(user.ProfileImageUrl)) rate += 20;
            if (user.IsGlutenFree || user.IsLactoseFree || user.IsPalmOilFree) rate += 20;

            user.ProfileCompletionRate = rate;

            // 🚀 GÜNCELLEME: Artık asenkron UpdateAsync kullanıyoruz
            await _userRepository.UpdateAsync(user);

            return _mapper.Map<UserProfileDto>(user);
        }

        public async Task UpdatePreferencesAsync(Guid id, UserPreferencesUpdateDto dto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) throw new Exception("Kullanıcı bulunamadı.");

            user.IsGlutenFree = dto.IsGlutenFree;
            user.IsLactoseFree = dto.IsLactoseFree;
            user.IsPalmOilFree = dto.IsPalmOilFree;

            // 🚀 GÜNCELLEME: Asenkron UpdateAsync
            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteUserAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) throw new Exception("Kullanıcı bulunamadı.");

            _userRepository.Remove(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task<UserProfileDto> HandleGoogleUserAsync(string email, string name, string pictureUrl)
        {
            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null)
            {
                user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = email,
                    FullName = name,
                    ProfileImageUrl = pictureUrl,
                    IsGoogleUser = true,
                    PasswordHash = "GOOGLE_AUTH",
                    ProfileCompletionRate = 60,
                    LastAiRequestDate = DateTime.UtcNow,
                    DailyAiRequestCount = 0
                };

                await _userRepository.AddAsync(user);
                await _userRepository.SaveChangesAsync();
            }

            return _mapper.Map<UserProfileDto>(user);
        }

        public async Task<IEnumerable<UserProfileDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserProfileDto>>(users);
        }

        public async Task<string> UploadProfileImageAsync(Guid userId, Stream fileStream, string fileName)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) throw new Exception("Kullanıcı bulunamadı.");

            var extension = Path.GetExtension(fileName).ToLower();
            var newFileName = $"user_{userId}_{DateTime.UtcNow.Ticks}{extension}";
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, newFileName);

            using (var newFileStream = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(newFileStream);
            }

            var fileUrl = $"/uploads/{newFileName}";
            user.ProfileImageUrl = fileUrl;

            // 🚀 GÜNCELLEME: Asenkron UpdateAsync
            await _userRepository.UpdateAsync(user);

            return fileUrl;
        }

        public async Task DeleteProfileImageAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) throw new Exception("Kullanıcı bulunamadı.");

            if (!string.IsNullOrEmpty(user.ProfileImageUrl))
            {
                string fileName = Path.GetFileName(user.ProfileImageUrl);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            user.ProfileImageUrl = null;

            // 🚀 GÜNCELLEME: Asenkron UpdateAsync
            await _userRepository.UpdateAsync(user);
        }
    }
}