using System.Collections.Generic;

namespace NutriZeka.Domain.Entities
{
    public class User : BaseEntity
    {

        public string Email { get; set; }
        public string PasswordHash { get; set; }

        public bool IsGoogleUser { get; set; }

        public string ProfileImageUrl { get; set; }

        public string FullName { get; set; }

        public int ProfileCompletionRate { get; set; } // Örn: 78


        public string PhoneNumber { get; set; }

        public bool IsGlutenFree { get; set; }
        public bool IsLactoseFree { get; set; }
        public bool IsPalmOilFree { get; set; }


        public bool Is2FAEnabled { get; set; }

        // --- İlişkiler ---
        public virtual ICollection<ScanHistory> ScanHistories { get; set; }
    }
}