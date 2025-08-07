using AutoMapper;

using MyApp.Application.DTOs;
using MyApp.Domain.Entities;

namespace MyApp.Application.Mappings;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryDto>().ReverseMap();
    }
}