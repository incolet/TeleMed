using Microsoft.AspNetCore.Mvc;
using TeleMed.DTOs;
using TeleMed.DTOs.Provider;
using TeleMed.Models;
using TeleMed.Repos.Abstracts;
using static TeleMed.Responses.CustomResponses;

namespace TeleMed.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProvidersController(IProvider providerRepo) : ControllerBase
    {
        [HttpPost]
        public ProviderResponse CreateProvider([FromBody] ProviderDto providerDto)
        {
            return providerRepo.CreateProvider(providerDto);
        }

        [HttpPut]
        public ProviderResponse UpdateProvider([FromBody] ProviderDto providerDto)
        {
            return providerRepo.UpdateProvider(providerDto);
        }

        [HttpDelete("{providerId}")]
        public ProviderResponse DeleteProvider(int providerId)
        {
            return providerRepo.DeleteProvider(providerId);
        }

        [HttpGet("{providerId}")]
        public Providers GetProvider(int providerId)
        {
            return providerRepo.GetProvider(providerId);
        }

        [HttpGet]
        public List<Providers> GetProviders()
        {
            return providerRepo.GetProviders();
        }
    }
}