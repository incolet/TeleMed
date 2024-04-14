using TeleMed.DTOs;
using static TeleMed.Responses.CustomResponses;

namespace TeleMed.Repos
{
    public interface IAccount
    {
        RegistrationResponse RegisterAsync(RegisterDTO model);
        LoginResponse LoginAsync(LoginDTO model);
    }
}
