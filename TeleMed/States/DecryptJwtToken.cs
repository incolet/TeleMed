using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TeleMed.DTOs.Auth;

namespace TeleMed.States;

public static class DecryptJwtToken
{
    
    public static CustomUserClaims DecryptToken(string jwtToken)
    {
        try
        {
            if (string.IsNullOrEmpty(jwtToken)) return new CustomUserClaims();

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwtToken);

            var name = token.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name);
            var email = token.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email);
            var role = token.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role);
            var uniqueId = token.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);

            return new CustomUserClaims(name!.Value, email!.Value, Convert.ToInt32(role!.Value), uniqueId!.Value);
        }
        catch
        {
            return null!;
        }
            
    }
}