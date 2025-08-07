using MyApp.Application.DTOs;

namespace MyApp.Application.Services.Interfaces;

public interface IUserService
{
    Task<UserDto> RegisterUserAsync(UserRegistrationDto registrationDto);
    Task<UserDto> AuthenticateUserAsync(string username, string password);
    Task<bool> AssignRoleToUserAsync(int userId, string roleName);
    Task<UserDto?> GetUserDetailsAsync(int userId);
}
