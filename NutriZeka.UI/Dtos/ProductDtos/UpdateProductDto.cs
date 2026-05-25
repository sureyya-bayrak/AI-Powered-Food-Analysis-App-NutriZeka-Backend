using System;
using System.ComponentModel.DataAnnotations;

namespace NutriZeka.WebUI.Dtos.ProductDtos
{
    public class UpdateProductDto
    {
        // Formda gizli (hidden) olarak tutulması için gereklidir, üzerinde hiçbir doğrulama (validation) etiketi olmamalıdır!
        public Guid Id { get; set; }

        // Doğrulama kuralları TAM OLARAK Barcode özelliğinin üstünde olmalıdır.
        [Required(ErrorMessage = "Barkod alanı zorunludur.")]
        [StringLength(13, ErrorMessage = "Barkod en fazla 13 karakter olabilir.")]
        public string Barcode { get; set; }

        [Required(ErrorMessage = "Ürünün Türkçe adı zorunludur.")]
        public string NameTr { get; set; }
        public string NameEn { get; set; }

        public string Brand { get; set; }
        public string Quantity { get; set; }
        public string ImageUrl { get; set; }

        public string NutriScoreGrade { get; set; }
        public string EcoScoreGrade { get; set; }
        public int NovaGroup { get; set; }

        public double EnergyKcal { get; set; }
        public double Fat { get; set; }
        public double Carbohydrates { get; set; }
        public double Sugars { get; set; }
        public double Proteins { get; set; }
        public double Salt { get; set; }

        public string IngredientsTextTr { get; set; }
        public string IngredientsTextEn { get; set; }
        public string AllergensTr { get; set; }
        public string AllergensEn { get; set; }

        public string CategoriesTr { get; set; }
        public string CategoriesEn { get; set; }

        public bool ContainsGluten { get; set; }
        public bool ContainsLactose { get; set; }
        public bool ContainsPalmOil { get; set; }

        public bool IsVerified { get; set; }
    }
}