using System;

namespace NutriZeka.Domain.Entities
{
    public class AIAnalysisCache : BaseEntity
    {
        // Yabancı Anahtarlar (Foreign Keys)
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public Guid ScanHistoryId { get; set; } // 🚀 ScanHistory ile ilişki!

        // Sorunun Türü (1: Health Profile Safe?, 2: Ingredients Explain, 3: Alternatives)
        public int QuestionType { get; set; }

        // O anki kullanıcı tercihleri (Eğer kullanıcı profilinden tercihini değiştirirse cache patlasın diye)
        public bool UserWasGlutenFree { get; set; }
        public bool UserWasLactoseFree { get; set; }
        public bool UserWasPalmOilFree { get; set; }

        // AI'ın verdiği ve saklayacağımız cevap
        public string AIResponse { get; set; }

        // --- Navigasyon Property'leri (İlişkiler) ---
        public virtual User User { get; set; }
        public virtual Product Product { get; set; }
        public virtual ScanHistory ScanHistory { get; set; }
    }
}