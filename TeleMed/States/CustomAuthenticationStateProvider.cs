using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using TeleMed.DTOs.Auth;

namespace TeleMed.States
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {

                if (string.IsNullOrEmpty(Constants.JwtToken))
                    return await Task.FromResult(new AuthenticationState(_anonymous));

                var getUserClaims = DecryptJwtToken.DecryptToken(Constants.JwtToken);
                if (string.IsNullOrEmpty(getUserClaims.Name) || string.IsNullOrEmpty(getUserClaims.Email))
                    return await Task.FromResult(new AuthenticationState(_anonymous));
                var claimsPrincipal = SetClaimPrincipal(getUserClaims);
                return await Task.FromResult(new AuthenticationState(claimsPrincipal));
            }
            catch
            {
                return await Task.FromResult(new AuthenticationState(_anonymous));
            }
        }

        public void UpdateAuthenticationState(string jwtToken)
        {
            var claimsPrincipal = new ClaimsPrincipal();
            if (!string.IsNullOrEmpty(jwtToken))
            {
                Constants.JwtToken = jwtToken;
                var getUserClaims = DecryptJwtToken.DecryptToken(jwtToken);
                claimsPrincipal = SetClaimPrincipal(getUserClaims);
            }
            else
            {
                Constants.JwtToken = string.Empty;
            }
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }

        private static ClaimsPrincipal SetClaimPrincipal(CustomUserClaims claims)
        {
            if (string.IsNullOrEmpty(claims.Email)) return new ClaimsPrincipal();
            return new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Email, claims.Email),
                new Claim(ClaimTypes.Name, claims.Name),
                new Claim(ClaimTypes.Role, claims.Role),
                new Claim(ClaimTypes.NameIdentifier, claims.UniqueId)
            }, "JwtAuth"));
        }

        public void Logout()
        {
            Constants.JwtToken = string.Empty;
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
        }
    }
}
