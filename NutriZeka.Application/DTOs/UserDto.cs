using System;
using System.ComponentModel.DataAnnotations;

namespace NutriZeka.Application.DTOs
{
    // Flutter'a (Dışarıya) göndereceğimiz güvenli profil modeli
    public class UserProfileDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string ProfileImageUrl { get; set; }
        public string PhoneNumber { get; set; }
        public int ProfileCompletionRate { get; set; }
        public bool IsGoogleUser { get; set; }
        public bool IsGlutenFree { get; set; }
        public bool IsLactoseFree { get; set; }
        public bool IsPalmOilFree { get; set; }
        public bool Is2FAEnabled { get; set; }
    }

    // Kullanıcı kayıt olurken Flutter'dan bize gelecek model
    public class UserRegisterDto
    {
        [Required(ErrorMessage = "Ad Soyad alanı zorunludur.")]
        public string FullName { get; set; } // 🚀 Bunu geri ekledik!
        [Required(ErrorMessage = "E-posta adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçersiz e-posta formatı.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [MinLength(8, ErrorMessage = "Şifre en az 8 karakter olmalıdır.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$",
            ErrorMessage = "Şifre en az bir büyük harf, bir küçük harf ve bir rakam içermelidir.")]
        public string Password { get; set; }

        // FullName alanı buradan silindi! 
    }

    // Kullanıcı giriş yaparken gelecek model
    public class UserLoginDto
    {
        [Required(ErrorMessage = "E-posta adresi zorunludur.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur.")]
        public string Password { get; set; }
    }
    // Profilin sadece temel bilgilerini güncellerken kullanılacak model
    public class UserUpdateDto
    {
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfileImageUrl { get; set; }
    }

    // Sadece diyet/filtre tercihlerini güncellerken kullanılacak model
    public class UserPreferencesUpdateDto
    {
        public bool IsGlutenFree { get; set; }
        public bool IsLactoseFree { get; set; }
        public bool IsPalmOilFree { get; set; }
    }
}