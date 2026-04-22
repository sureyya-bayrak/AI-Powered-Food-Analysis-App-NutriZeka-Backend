using System.Collections.Generic;

namespace NutriZeka.Domain.Entities
{
    public class Product : BaseEntity
    {
        // --- Kimlik ve Marka ---
        public string Barcode { get; set; }
        public string NameTr { get; set; }
        public string NameEn { get; set; }
        public string Brand { get; set; }
        public string Quantity { get; set; }
        public string ImageUrl { get; set; }
        public string Categories { get; set; }

        public bool IsVerified { get; set; }

        // --- Skorlar ---
        public string NutriScoreGrade { get; set; }
        public string EcoScoreGrade { get; set; }
        public int NovaGroup { get; set; }

        // --- Metin Alanları (Senin istediğin dinamik yapı) ---
        public string IngredientsTextTr { get; set; }
        public string IngredientsTextEn { get; set; }

        // Alerjenleri burada virgülle ayrılmış "Süt, Yer Fıstığı, Gluten" gibi tutacağız
        public string AllergensTr { get; set; }
        public string AllergensEn { get; set; }

        // --- Temel Besin Değerleri (Dashboard grafikleri için) ---
        public double EnergyKcal { get; set; }
        public double Fat { get; set; }
        public double Carbohydrates { get; set; }
        public double Sugars { get; set; }
        public double Proteins { get; set; }
        public double Salt { get; set; }

        // --- İlişkiler ---
        public virtual ICollection<ScanHistory> ScanHistories { get; set; }
    }
}