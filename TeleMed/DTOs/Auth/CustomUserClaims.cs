namespace TeleMed.DTOs.Auth
{
    public record CustomUserClaims(string Name=null!, string Email =null!, string Role = null!, string UniqueId = null!);
    
}
