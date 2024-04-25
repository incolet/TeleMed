using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Xml;
using TeleMed.Common.Extensions;
using TeleMed.DTOs.Auth;
using TeleMed.Responses;
using TeleMed.States;
using Formatting = Newtonsoft.Json.Formatting;
using API = TeleMed.States.ApiEndpoints;

namespace TeleMed.Services
{
    
    public abstract class BaseService<T>(
        ILogger<T> logger,
        HttpClient httpClient,
        IServiceProvider serviceProvider = null!)
    {
        protected IServiceProvider ServiceProvider = serviceProvider;
        protected readonly HttpClient HttpClient = httpClient;

        //protected readonly string BaseUrl = $"api/{typeof(T).Name.Replace("Service", string.Empty)}";

        public async Task<HttpResponseMessage> SendRequestAsync<TBody>(HttpRequestMessage request, TBody body, bool attachToken)
        {
            var response = await HttpClient.ExtSendRequestAsync(request,body, attachToken);

            if (response.StatusCode != System.Net.HttpStatusCode.Unauthorized) return response;
            
            // Call the refreshTokenMethod to refresh the token
            await RefreshToken();
            // Retry the request with the new token
            response = await HttpClient.ExtSendRequestAsync(request,body,attachToken);

            return response;
        }
        private async Task RefreshToken()
        {
            var response = await HttpClient.PostAsJsonAsync($"{API.AccountsApi}/refresh-token", new UserSession()
            {
                JwtToken = Constants.JwtToken
            });
            var result = await response.Content.ReadFromJsonAsync<CustomResponses.LoginResponse>();
            Constants.JwtToken = result!.JWTToken;
        }
        
        protected void LogInformation(string message, object data)
        {
            logger.LogInformation($"{message}: \n{JsonConvert.SerializeObject(data, Formatting.Indented)}\n");
        }

        protected void LogError(Exception ex, string message)
        {
            logger.LogError(ex, $"\n {message}");
        }

        

    }
}
