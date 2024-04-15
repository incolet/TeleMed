using TeleMed.DTOs;
using TeleMed.Models;
using static TeleMed.Responses.CustomResponses;

namespace TeleMed.Repos.Abstracts;

public interface IAccount
{
    (RegistrationResponse,int) RegisterAsync(RegisterDTO model);
    LoginResponse LoginAsync(LoginDTO model);
    ApplicationUser GetUser(string email);
    ApplicationUser GetUser(int id);
}

