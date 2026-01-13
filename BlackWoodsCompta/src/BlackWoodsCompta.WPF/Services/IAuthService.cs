using BlackWoodsCompta.Models.DTOs;
using BlackWoodsCompta.Models.Entities;

namespace BlackWoodsCompta.WPF.Services;

public interface IAuthService
{
    User? CurrentUser { get; }
    bool IsAuthenticated { get; }
    string? Token { get; }
    Task<LoginResponse> LoginAsync(string username, string password);
    void Logout();
    bool HasRole(string role);
}
