using BlackWoodsCompta.Models.DTOs;
using BlackWoodsCompta.Models.Entities;
using BlackWoodsCompta.Models.Enums;
using Serilog;

namespace BlackWoodsCompta.WPF.Services;

public class AuthService : IAuthService
{
    private readonly IDataService _dataService;
    
    public User? CurrentUser { get; private set; }
    public bool IsAuthenticated => CurrentUser != null;
    public string? Token { get; private set; }

    public AuthService(IDataService dataService)
    {
        _dataService = dataService;
    }

    public async Task<LoginResponse> LoginAsync(string username, string password)
    {
        try
        {
            var response = await _dataService.LoginAsync(username, password);

            if (response != null && response.Success && response.User != null)
            {
                Token = response.Token;
                
                CurrentUser = new User
                {
                    Id = response.User.Id,
                    Username = response.User.Username,
                    FullName = response.User.FullName,
                    Role = Enum.Parse<UserRole>(response.User.Role),
                    Discord = response.User.Discord
                };

                Log.Information($"User {username} logged in successfully");
                return response;
            }

            return new LoginResponse
            {
                Success = false,
                Message = response?.Message ?? "Erreur de connexion"
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Login error");
            return new LoginResponse
            {
                Success = false,
                Message = "Erreur de connexion"
            };
        }
    }

    public void Logout()
    {
        CurrentUser = null;
        Token = null;
        Log.Information("User logged out");
    }

    public bool HasRole(string role)
    {
        if (!IsAuthenticated) return false;
        return CurrentUser!.Role.ToString().Equals(role, StringComparison.OrdinalIgnoreCase);
    }
}
