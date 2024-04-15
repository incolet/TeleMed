using TeleMed.DTOs;
using TeleMed.DTOs.Auth;
using TeleMed.Models;
using static TeleMed.Responses.CustomResponses;

namespace TeleMed.Repos.Abstracts;

public interface IAccount
{
    (RegistrationResponse,int) RegisterAsync(RegisterDto model);
    LoginResponse LoginAsync(LoginDTO model);
    ApplicationUser GetUser(string email);
    ApplicationUser GetUser(int id);
    LoginResponse RefreshToken(UserSession userSession);
}

