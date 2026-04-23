using AutoMapper;
using NutriZeka.Application.DTOs;
using NutriZeka.Domain.Entities;

namespace NutriZeka.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Bu satır "Product görürsen onu ProductDetailDto'ya çevirebilirsin" demek.
            // .ReverseMap() ise tam tersini de (DTO'dan Entity'ye) yapabilmeni sağlar.
            CreateMap<Product, ProductDetailDto>().ReverseMap();
        }
    }
}