using AutoMapper;
using NutriZeka.Application.DTOs;
using NutriZeka.Domain.Entities;

namespace NutriZeka.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // --- Product Mappings ---
            CreateMap<Product, ProductDetailDto>().ReverseMap();
            CreateMap<Product, ProductCreateDto>().ReverseMap();
            CreateMap<Product, ProductUpdateDto>().ReverseMap();

            // --- ScanHistory Mappings ---
            CreateMap<ScanHistory, ScanHistoryDto>()
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.AIAnalysisSummary, opt => opt.MapFrom(src => src.AIAnalysisSummary))
                .ForMember(dest => dest.Source, opt => opt.MapFrom(src => src.Source))

                // Çok Dilli İsimler
                .ForMember(dest => dest.NameTr, opt => opt.MapFrom(src => src.Product != null ? src.Product.NameTr : string.Empty))
                .ForMember(dest => dest.NameEn, opt => opt.MapFrom(src => src.Product != null ? src.Product.NameEn : string.Empty))

                // Temel Ürün Bilgileri
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.NameTr : string.Empty))
                .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.Product != null ? src.Product.Brand : string.Empty))
                .ForMember(dest => dest.Barcode, opt => opt.MapFrom(src => src.Product != null ? src.Product.Barcode : string.Empty))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Product != null ? src.Product.ImageUrl : string.Empty))

                // 🚀 YENİ EKLENEN MAKROLAR VE DETAYLAR (CSV İÇİN)
                .ForMember(dest => dest.Proteins, opt => opt.MapFrom(src => src.Product != null ? src.Product.Proteins : 0.0))
                .ForMember(dest => dest.Carbohydrates, opt => opt.MapFrom(src => src.Product != null ? src.Product.Carbohydrates : 0.0))
                .ForMember(dest => dest.Salt, opt => opt.MapFrom(src => src.Product != null ? src.Product.Salt : 0.0))

                // 🚀 DİYET FİLTRELERİ VE ANALİZLER
                .ForMember(dest => dest.ContainsGluten, opt => opt.MapFrom(src => src.Product != null ? src.Product.ContainsGluten : false))
                .ForMember(dest => dest.ContainsLactose, opt => opt.MapFrom(src => src.Product != null ? src.Product.ContainsLactose : false))
                .ForMember(dest => dest.ContainsPalmOil, opt => opt.MapFrom(src => src.Product != null ? src.Product.ContainsPalmOil : false))

                // =================================================================
                // 🚀 DÜZELTİLEN KISIM: ARAYÜZ (UI) İÇİN MEVCUT ALANLAR KORUNDU
                // =================================================================
                .ForMember(dest => dest.DisplayIngredients, opt => opt.MapFrom(src => src.Product != null ? src.Product.IngredientsTextTr : string.Empty))
                .ForMember(dest => dest.DisplayAllergens, opt => opt.MapFrom(src => src.Product != null ? src.Product.AllergensTr : string.Empty))
                .ForMember(dest => dest.DisplayCategory, opt => opt.MapFrom(src => src.Product != null ? src.Product.CategoriesTr : string.Empty))

                // =================================================================
                // 🚀 YENİ EKLENEN KISIM: CSV RAPORU VE ÇOK DİLLİLİK İÇİN
                // =================================================================
                .ForMember(dest => dest.IngredientsTextTr, opt => opt.MapFrom(src => src.Product != null ? src.Product.IngredientsTextTr : string.Empty))
                .ForMember(dest => dest.IngredientsTextEn, opt => opt.MapFrom(src => src.Product != null ? src.Product.IngredientsTextEn : string.Empty))

                .ForMember(dest => dest.AllergensTr, opt => opt.MapFrom(src => src.Product != null ? src.Product.AllergensTr : string.Empty))
                .ForMember(dest => dest.AllergensEn, opt => opt.MapFrom(src => src.Product != null ? src.Product.AllergensEn : string.Empty))

                .ForMember(dest => dest.CategoriesTr, opt => opt.MapFrom(src => src.Product != null ? src.Product.CategoriesTr : string.Empty))
                .ForMember(dest => dest.CategoriesEn, opt => opt.MapFrom(src => src.Product != null ? src.Product.CategoriesEn : string.Empty))

                // Rozetler ve Temel Değerler
                .ForMember(dest => dest.NutriScoreGrade, opt => opt.MapFrom(src => src.Product != null ? src.Product.NutriScoreGrade : string.Empty))
                .ForMember(dest => dest.NovaGroup, opt => opt.MapFrom(src => src.Product != null ? src.Product.NovaGroup : 0))
                .ForMember(dest => dest.EnergyKcal, opt => opt.MapFrom(src => src.Product != null ? src.Product.EnergyKcal : 0.0))
                .ForMember(dest => dest.Fat, opt => opt.MapFrom(src => src.Product != null ? src.Product.Fat : 0.0))
                .ForMember(dest => dest.Sugars, opt => opt.MapFrom(src => src.Product != null ? src.Product.Sugars : 0.0));

            // --- Diğer Mappings ---
            CreateMap<ScanHistoryCreateDto, ScanHistory>();
            CreateMap<ScanHistoryUpdateDto, ScanHistory>();
            CreateMap<User, UserProfileDto>().ReverseMap();
            CreateMap<UserUpdateDto, User>();
        }
    }
}