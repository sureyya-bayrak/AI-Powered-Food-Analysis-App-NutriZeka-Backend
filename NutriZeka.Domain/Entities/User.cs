using System.Collections.Generic;

namespace NutriZeka.Domain.Entities
{
    public class User : BaseEntity
    {
        // Zorunlu alanlar (Kayıt anında dolacak)
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        // Null geçilebilen (Opsiyonel) alanlar - Yanındaki '?' işaretine dikkat!
        public string? FullName { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? PhoneNumber { get; set; }

        // Default olarak 0 veya false gelecek alanlar
        public int ProfileCompletionRate { get; set; } = 0;
        public bool IsGoogleUser { get; set; } = false;
        public bool Is2FAEnabled { get; set; } = false;

        // Sağlık Tercihleri - Default olarak false (Seçilmemiş) başlar
        public bool IsGlutenFree { get; set; } = false;
        public bool IsLactoseFree { get; set; } = false;
        public bool IsPalmOilFree { get; set; } = false;
        public int DailyAiRequestCount { get; set; } = 0;
        public DateTime LastAiRequestDate { get; set; } = DateTime.UtcNow;

        // --- İlişkiler ---
        public virtual ICollection<ScanHistory> ScanHistories { get; set; } = new List<ScanHistory>();
    }
}