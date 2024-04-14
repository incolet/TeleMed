using TeleMed.DTOs;
using static TeleMed.Responses.CustomResponses;

namespace TeleMed.Services
{
    public interface IAccountService
    {
        Task<RegistrationResponse> RegisterAsync(RegisterDTO model);
        Task<LoginResponse> LoginAsync(LoginDTO model);
    }
}
