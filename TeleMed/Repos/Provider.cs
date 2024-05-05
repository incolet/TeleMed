using TeleMed.Data;
using TeleMed.Data.Abstracts;
using TeleMed.DTOs.Auth;
using TeleMed.DTOs.Provider;
using TeleMed.Models;
using TeleMed.Repos.Abstracts;
using TeleMed.Responses;
using TeleMed.States;

namespace TeleMed.Repos;

public class Provider (IAccount accountRepo, IAppDbContext appDbContext)
    : IProvider
{
    public CustomResponses.ProviderResponse CreateProvider(ProviderDto providerDto)
    {
        try
        {
            var loginDto = new LoginDto
            {
                Email = providerDto.Email
            };
            var findUser = accountRepo.GetUser(loginDto);
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (findUser is not null)
                return (new CustomResponses.ProviderResponse(false, "User already exist"));
            
            //Create User in the database
            var registerResponse = accountRepo.RegisterAsync(new RegisterDto
            {
                Email = providerDto.Email,
                Password = providerDto.LastName,
                Name = providerDto.FirstName,
                Role = (int)UserRoles.Provider
            });
            
            if (!registerResponse.Item1.Flag || registerResponse.Item2 == 0)
            {
                return new CustomResponses.ProviderResponse
                {
                    Flag = false,
                    Message = "Unable to create provider"
                };
            }
            
            //Create Provider in the database
            var newProvider = new Providers
            {
                UserId = registerResponse.Item2,
                FirstName = providerDto.FirstName,
                LastName = providerDto.LastName,
                Email = providerDto.Email,
                Phone = providerDto.Phone
            };
            
            appDbContext.Providers.Add(newProvider);
            appDbContext.SaveChanges();
            
            return new CustomResponses.ProviderResponse
            {
                Flag = true,
                Message = "Provider created successfully"
            };

        }
        catch (Exception e)
        {
            // Log error
            return new CustomResponses.ProviderResponse
            {
                Flag = false,
                Message = "An error occurred"
            };
        }
    }

    public CustomResponses.ProviderResponse UpdateProvider(ProviderDto providerDto)
    {
        var provider = GetProvider(providerDto.Id);
        if (provider.Id == 0)
        {
            return new CustomResponses.ProviderResponse
            {
                Flag = false,
                Message = "Provider not found"
            };
        }
        
        provider.FirstName = providerDto.FirstName;
        provider.LastName = providerDto.LastName;
        provider.Email = providerDto.Email;
        provider.Phone = providerDto.Phone;
        
        appDbContext.Providers.Update(provider);
        appDbContext.SaveChanges();
        
        return new CustomResponses.ProviderResponse
        {
            Flag = true,
            Message = "Provider updated successfully"
        };
    }

    public CustomResponses.ProviderResponse DeleteProvider(int providerId)
    {
        var provider = GetProvider(providerId);
        if (provider.Id == 0)
        {
            return new CustomResponses.ProviderResponse
            {
                Flag = false,
                Message = "Provider not found"
            };
        }
        
        provider.Status = false;
        
        appDbContext.Providers.Update(provider);
        appDbContext.SaveChanges();
        
        return new CustomResponses.ProviderResponse
        {
            Flag = true,
            Message = "Provider deleted successfully"
        };
    }

    public Providers GetProvider(int providerId)
    {
        var provider = appDbContext.Providers.FirstOrDefault(e => e.Id == providerId);
        return provider ?? new Providers();
    }

    public List<Providers> GetProviders()
    {
        return appDbContext.Providers.ToList();
    }
}