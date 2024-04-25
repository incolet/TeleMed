using TeleMed.Common.Extensions;
using TeleMed.DTOs.Provider;
using TeleMed.Models;
using TeleMed.Responses;
using TeleMed.Services.Abstracts;

using API = TeleMed.States.ApiEndpoints;
namespace TeleMed.Services;

public class ProvidersService(ILogger<ProvidersService> logger, HttpClient httpClient, IServiceProvider serviceProvider)
    : BaseService<ProvidersService> (logger,httpClient,serviceProvider), IProvidersService
{
    private readonly string _baseUrl = API.ProvidersApi;


    public async Task<CustomResponses.ProviderResponse> CreateProvider(ProviderDto providerDto)
    {
        var response = await HttpClient.ExtSendRequestAsync(new HttpRequestMessage(HttpMethod.Post,$"{_baseUrl}"),providerDto);
        var result = await response.Content.ReadFromJsonAsync<CustomResponses.ProviderResponse>();
        return result!;
    }

    public async Task<CustomResponses.ProviderResponse> UpdateProvider(ProviderDto providerDto)
    {
        var response = await HttpClient.ExtSendRequestAsync(new HttpRequestMessage(HttpMethod.Put,$"{_baseUrl}"),providerDto);
        var result = await response.Content.ReadFromJsonAsync<CustomResponses.ProviderResponse>();
        return result!;
    }

    public async Task<CustomResponses.ProviderResponse> DeleteProvider(int providerId)
    {
        var response = await HttpClient.ExtSendRequestAsyncNoBody(new HttpRequestMessage(HttpMethod.Delete,$"{_baseUrl}/{providerId}"));
        var result = await response.Content.ReadFromJsonAsync<CustomResponses.ProviderResponse>();
        return result!;
    }

    public async Task<Providers> GetProvider(int providerId)
    {
        var response = await HttpClient.ExtSendRequestAsyncNoBody(new HttpRequestMessage(HttpMethod.Get,$"{_baseUrl}/{providerId}"));
        var result = await response.Content.ReadFromJsonAsync<Providers>();
        return result!;
    }

    public async Task<List<Providers>> GetProviders()
    {
        var response = await HttpClient.ExtSendRequestAsyncNoBody(new HttpRequestMessage(HttpMethod.Get,$"{_baseUrl}"));
        var result = await response.Content.ReadFromJsonAsync<List<Providers>>();
        return result!;
    }
}