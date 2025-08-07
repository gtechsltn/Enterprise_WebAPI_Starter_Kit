using AutoMapper;

using MyApp.Application.DTOs;
using MyApp.Domain.Entities;

namespace MyApp.Application.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>().ReverseMap();
    }
}
