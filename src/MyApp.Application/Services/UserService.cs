using AutoMapper;

using MyApp.Application.DTOs;
using MyApp.Application.Interfaces;
using MyApp.Application.Services.Interfaces;
using MyApp.Domain.Entities;

namespace MyApp.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;
    public UserService(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, IMapper mapper)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserDto> RegisterUserAsync(UserRegistrationDto registrationDto)
    {
        // Kiểm tra tính duy nhất của email
        if (await _unitOfWork.Users.IsEmailUniqueAsync(registrationDto.Email))
        {
            throw new InvalidOperationException("Email already exists.");
        }

        // Tạo và lưu người dùng mới
        var user = new User
        {
            Username = registrationDto.Username,
            Email = registrationDto.Email,
            PasswordHash = _passwordHasher.HashPassword(registrationDto.Password),
            FullName = registrationDto.FullName,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.CompleteAsync();

        // Ánh xạ sang DTO và trả về
        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> AuthenticateUserAsync(string username, string password)
    {
        var user = await _unitOfWork.Users.GetUserByUsernameAsync(username);

        if (user == null || !_passwordHasher.VerifyPassword(password, user.PasswordHash))
        {
            throw new InvalidOperationException("Invalid username or password.");
        }

        return _mapper.Map<UserDto>(user);
    }

    public async Task<bool> AssignRoleToUserAsync(int userId, string roleName)
    {
        // Tìm kiếm người dùng và role
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        var role = await _unitOfWork.Roles.GetRoleByNameAsync(roleName);

        if (user == null || role == null)
        {
            return false;
        }

        // Gán role cho người dùng
        var userRole = new UserRole
        {
            UserId = user.Id,
            RoleId = role.Id
        };

        // EF Core sẽ tự động thêm UserRole vào bảng nối
        // thông qua mối quan hệ được cấu hình.
        user.UserRoles.Add(userRole);
        await _unitOfWork.CompleteAsync();

        return true;
    }

    public async Task<UserDto?> GetUserDetailsAsync(int userId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);

        if (user == null) return null;

        return _mapper.Map<UserDto>(user);
    }
}
