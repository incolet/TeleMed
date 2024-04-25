using TeleMed.DTOs;
using TeleMed.DTOs.Provider;
using TeleMed.Models;
using static TeleMed.Responses.CustomResponses;

namespace TeleMed.Repos.Abstracts;

public interface IProvider
{
    ProviderResponse CreateProvider(ProviderDto providerDto);
    ProviderResponse UpdateProvider(ProviderDto providerDto);
    ProviderResponse DeleteProvider(int providerId);
    Providers GetProvider(int providerId);
    List<Providers> GetProviders();
}