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

        // --- Kategoriler (Öneri sistemi için çift dilli hale getirdik) ---
        public string CategoriesTr { get; set; }
        public string CategoriesEn { get; set; }

        public bool IsVerified { get; set; }

        // --- Skorlar ---
        public string NutriScoreGrade { get; set; }
        public string EcoScoreGrade { get; set; }
        public int NovaGroup { get; set; }

        // --- İçindekiler ---
        public string IngredientsTextTr { get; set; }
        public string IngredientsTextEn { get; set; }

        // --- Alerjenler (Virgülle ayrılmış metin) ---
        public string AllergensTr { get; set; }
        public string AllergensEn { get; set; }

        // --- Sağlık ve Diyet Filtreleri (Mobil tarafta hızlı sorgu için) ---
        public bool ContainsGluten { get; set; }
        public bool ContainsLactose { get; set; }
        public bool ContainsPalmOil { get; set; }

        // --- Temel Besin Değerleri ---
        public double EnergyKcal { get; set; }
        public double Fat { get; set; }
        public double Carbohydrates { get; set; }
        public double Sugars { get; set; }
        public double Proteins { get; set; }
        public double Salt { get; set; }
    }
}