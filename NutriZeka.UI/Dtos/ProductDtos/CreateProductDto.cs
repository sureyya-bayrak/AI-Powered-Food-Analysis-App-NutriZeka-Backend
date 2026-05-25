using System.ComponentModel.DataAnnotations;

namespace NutriZeka.WebUI.Dtos.ProductDtos
{
    public class CreateProductDto
    {
        [Required(ErrorMessage = "Barkod alanı zorunludur.")]
        [StringLength(13, ErrorMessage = "Barkod en fazla 13 karakter olabilir.")]
        public string Barcode { get; set; }

        [Required(ErrorMessage = "Ürünün Türkçe adı zorunludur.")]
        public string NameTr { get; set; }

        public string NameEn { get; set; }
        public string Brand { get; set; }
        public string Quantity { get; set; }
        public string ImageUrl { get; set; }

        // --- Skorlar ---
        public string NutriScoreGrade { get; set; }
        public string EcoScoreGrade { get; set; }
        public int NovaGroup { get; set; }

        // --- Besin Değerleri (Double tipinde) ---
        public double EnergyKcal { get; set; }
        public double Fat { get; set; }
        public double Carbohydrates { get; set; }
        public double Sugars { get; set; }
        public double Proteins { get; set; }
        public double Salt { get; set; }

        // --- İçerik ve Alerjenler ---
        public string IngredientsTextTr { get; set; }
        public string IngredientsTextEn { get; set; }
        public string AllergensTr { get; set; }
        public string AllergensEn { get; set; }

        // --- Kategoriler ---
        public string CategoriesTr { get; set; }
        public string CategoriesEn { get; set; }

        // --- Sağlık Filtreleri ---
        public bool ContainsGluten { get; set; }
        public bool ContainsLactose { get; set; }
        public bool ContainsPalmOil { get; set; }

        public bool IsVerified { get; set; }
    }
}