using System;

namespace NutriZeka.Application.DTOs
{
    public class ProductDetailDto
    {
        public Guid Id { get; set; }
        public string Barcode { get; set; }

        // --- Dinamik Alanlar ---
        public string DisplayName { get; set; }
        public string DisplayIngredients { get; set; }
        public string DisplayAllergens { get; set; }
        public string DisplayCategory { get; set; }

        // --- Çok Dilli İsim ve Marka ---
        public string NameTr { get; set; }
        public string NameEn { get; set; }
        public string Brand { get; set; }
        public string Quantity { get; set; }
        public string ImageUrl { get; set; }
        public bool IsVerified { get; set; }

        // --- All Scores ---
        public string NutriScoreGrade { get; set; }
        public int NovaGroup { get; set; }
        public string EcoScoreGrade { get; set; }

        // --- EKSİKLİK BURADAYDI: Kategori Kaynakları eklendi ---
        public string CategoriesTr { get; set; }
        public string CategoriesEn { get; set; }

        // --- Insights & Ingredients (TR/EN) ---
        public string IngredientsTextTr { get; set; }
        public string IngredientsTextEn { get; set; }
        public string AllergensTr { get; set; }
        public string AllergensEn { get; set; }

        // --- Sağlık ve Diyet Filtreleri ---
        public bool ContainsGluten { get; set; }
        public bool ContainsLactose { get; set; }
        public bool ContainsPalmOil { get; set; }

        // --- Nutrition Facts (100g için) ---
        public double EnergyKcal { get; set; }
        public double Fat { get; set; }
        public double Carbohydrates { get; set; }
        public double Sugars { get; set; }
        public double Proteins { get; set; }
        public double Salt { get; set; }
    }
}