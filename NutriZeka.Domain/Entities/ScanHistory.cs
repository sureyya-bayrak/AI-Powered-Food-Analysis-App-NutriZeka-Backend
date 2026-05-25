using System;

namespace NutriZeka.Domain.Entities
{
    public class ScanHistory : BaseEntity
    {
        // --- İlişkiler ---
        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; }

        // --- Durum İşaretleyicileri (Flags) ---

        // Tasarımdaki kalp ikonu için
        public bool IsFavorite { get; set; }

        // Tasarımdaki "Save" (Kaydet) işlevi için
        public bool IsSaved { get; set; }

        // --- Analiz Verisi ---

        // Yapay zekanın o tarama anında ürettiği özel yorum
        public string? AIAnalysisSummary { get; set; }

        // --- FİLTRELEME İÇİN EKLENDİ ---
        // Ürünün arama ekranından mı (search) yoksa tarama ekranından mı (scan) eklendiğini tutar
        public string Source { get; set; } = string.Empty;

        // 🚀 YENİ EKLENEN İLİŞKİ: Bu tarama işlemine ait AI analizleri
        public virtual ICollection<AIAnalysisCache> AIAnalyses { get; set; } = new List<AIAnalysisCache>();
    }
}