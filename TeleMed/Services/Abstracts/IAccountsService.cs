using TeleMed.DTOs;
using TeleMed.DTOs.Auth;
using static TeleMed.Responses.CustomResponses;

namespace TeleMed.Services.Abstracts;

public interface IAccountsService
{
    Task<RegistrationResponse> RegisterAsync(RegisterDto model);
    Task<LoginResponse> LoginAsync(LoginDTO model);
    Task<LoginResponse> RefreshToken(UserSession userSession);
}

