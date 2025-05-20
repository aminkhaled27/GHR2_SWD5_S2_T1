using AutoMapper;
using BusinessLayer.DTO;
using BusinessLayer.DTO.Product;
using DataAccessLayer.Entities;

namespace BusinessLayer.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductCardDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));

            CreateMap<Product, FeaturedProductDTO>();
        }
    }
} 