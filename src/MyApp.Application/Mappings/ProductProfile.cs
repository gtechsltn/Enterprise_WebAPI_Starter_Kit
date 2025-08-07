using AutoMapper;

using MyApp.Application.DTOs;
using MyApp.Domain.Entities;

namespace MyApp.Application.Mappings;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductDto>().ReverseMap();
    }
}
