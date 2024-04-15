using TeleMed.Common.Extensions;
using TeleMed.DTOs;
using TeleMed.DTOs.Auth;
using TeleMed.Responses;
using TeleMed.Services.Abstracts;
using static TeleMed.Responses.CustomResponses;

namespace TeleMed.Services
{
    public class AccountsService(ILogger<AccountsService> logger,HttpClient httpClient, IServiceProvider serviceProvider)
        : BaseService<AccountsService>(logger,httpClient, serviceProvider), IAccountsService
    {
       public async Task<LoginResponse> LoginAsync(LoginDTO model)
        {
            var response = await HttpClient.SendRequestAsync(new HttpRequestMessage(HttpMethod.Post,$"{BaseUrl}/login"),model, false);
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            return result!;
        }

        public async Task<LoginResponse> RefreshToken(UserSession userSession)
        {
            var response = await HttpClient.PostAsJsonAsync($"{BaseUrl}/refresh-token", userSession);
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            return result!;
        }

        public async Task<RegistrationResponse> RegisterAsync(RegisterDto model)
        {
            var response = await HttpClient.PostAsJsonAsync($"{BaseUrl}/register", model);
            var result = await response.Content.ReadFromJsonAsync<RegistrationResponse>();
            return result!;
        }
    }
}
