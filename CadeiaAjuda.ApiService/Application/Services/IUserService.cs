using CadeiaAjuda.ApiService.Application.DTOs;

namespace CadeiaAjuda.ApiService.Application.Services;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<UserDto?> GetByIdAsync(Guid id);
    Task<UserDto> CreateAsync(UserCreateDto dto);
    Task<UserDto?> UpdateAsync(UserUpdateDto dto);
    Task<bool> ToggleActiveAsync(Guid id);
    Task<UserDto?> AuthenticateAsync(LoginDto dto);
}
