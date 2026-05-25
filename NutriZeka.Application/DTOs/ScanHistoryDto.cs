using System;

namespace NutriZeka.Application.DTOs
{
    public class ScanHistoryDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid UserId { get; set; }

        // --- Ürün Temel Bilgileri ---
        public string ProductName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string NameTr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Barcode { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;

        // --- Rozetler ve Kategori ---
        public string NutriScoreGrade { get; set; } = string.Empty;
        public int NovaGroup { get; set; }
        public string DisplayCategory { get; set; } = string.Empty; // 🚀 Eklendi

        // --- Nutrition Facts (Detaylı Rapor İçin) ---
        public double EnergyKcal { get; set; }
        public double Fat { get; set; }
        public double Sugars { get; set; }
        public double Proteins { get; set; } // 🚀 Eklendi
        public double Carbohydrates { get; set; } // 🚀 Eklendi
        public double Salt { get; set; } // 🚀 Eklendi

        // --- Diyet ve Sağlık Analizi (Filtreler) ---
        public bool ContainsGluten { get; set; } // 🚀 Eklendi
        public bool ContainsLactose { get; set; } // 🚀 Eklendi
        public bool ContainsPalmOil { get; set; } // 🚀 Eklendi

        // --- İçindekiler ve Alerjenler (CSV'de tam metin görmek için) ---
        public string DisplayIngredients { get; set; } = string.Empty; // 🚀 Eklendi
        public string DisplayAllergens { get; set; } = string.Empty; // 🚀 Eklendi
                                                                     // --- 🚀 CSV RAPORU VE DİL UYUMU İÇİN YANLARINA EKLENEN YENİ ALANLAR ---
        public string CategoriesTr { get; set; } = string.Empty;
        public string CategoriesEn { get; set; } = string.Empty;
        public string IngredientsTextTr { get; set; } = string.Empty;
        public string IngredientsTextEn { get; set; } = string.Empty;
        public string AllergensTr { get; set; } = string.Empty;
        public string AllergensEn { get; set; } = string.Empty;

        // --- Geçmiş ve Durum Yönetimi ---
        public bool IsFavorite { get; set; }
        public bool IsSaved { get; set; }
        public string? AIAnalysisSummary { get; set; }
        public string Source { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }
}