using TeleMed.DTOs;
using TeleMed.Responses;
using TeleMed.Services.Abstracts;
using static TeleMed.Responses.CustomResponses;

namespace TeleMed.Services
{
    public class AccountService : BaseService<AccountService>, IAccountService
    {
        private readonly HttpClient _httpClient;
        public AccountService(HttpClient httpClient, ILogger<AccountService> logger, IServiceProvider serviceProvider) 
            : base(logger,serviceProvider)
        {
            this._httpClient = httpClient;
        }
        public async Task<LoginResponse> LoginAsync(LoginDTO model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Accounts/login", model);
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            return result!;
        }

        public async Task<RegistrationResponse> RegisterAsync(RegisterDTO model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Accounts/register", model);
            var result = await response.Content.ReadFromJsonAsync<RegistrationResponse>();
            return result!;
        }
    }
}
