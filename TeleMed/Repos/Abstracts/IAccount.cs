using TeleMed.DTOs;
using TeleMed.DTOs.Auth;
using TeleMed.Models;
using static TeleMed.Responses.CustomResponses;

namespace TeleMed.Repos.Abstracts;

public interface IAccount
{
    (RegistrationResponse,int) RegisterAsync(RegisterDto model);
    LoginResponse LoginAsync(LoginDto model);
    ApplicationUser GetUser(LoginDto model);
    ApplicationUser GetUser(int id);
    LoginResponse RefreshToken(UserSession userSession);
    bool IsUserInRole(ApplicationUser user, int role);
}

