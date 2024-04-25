using TeleMed.DTOs.Provider;
using TeleMed.Models;
using TeleMed.Responses;

namespace TeleMed.Services.Abstracts;

public interface IProvidersService
{
    Task<CustomResponses.ProviderResponse> CreateProvider(ProviderDto providerDto);
    Task<CustomResponses.ProviderResponse> UpdateProvider(ProviderDto providerDto);
    Task<CustomResponses.ProviderResponse> DeleteProvider(int providerId);
    Task<Providers> GetProvider(int providerId);
    Task<List<Providers>> GetProviders();
    
}