using TeleMed.Common.Extensions;
using TeleMed.DTOs.Auth;
using TeleMed.Services.Abstracts;
using static TeleMed.Responses.CustomResponses;
using API = TeleMed.States.ApiEndpoints;

namespace TeleMed.Services
{
    public class AccountsService(ILogger<AccountsService> logger,HttpClient httpClient, IServiceProvider serviceProvider)
        : BaseService<AccountsService>(logger,httpClient, serviceProvider), IAccountsService
    {
        private readonly string _baseUrl = API.AccountsApi;
        
       public async Task<LoginResponse> LoginAsync(LoginDto model)
        {
            var response = await HttpClient.ExtSendRequestAsync(new HttpRequestMessage(HttpMethod.Post,$"{_baseUrl}/login"),model, false);
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            return result!;
        }
       
        public async Task<RegistrationResponse> RegisterAsync(RegisterDto model)
        {
            var response = await HttpClient.PostAsJsonAsync($"{_baseUrl}/register", model);
            var result = await response.Content.ReadFromJsonAsync<RegistrationResponse>();
            return result!;
        }
        
    }
}
