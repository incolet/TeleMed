namespace TeleMed.DTOs.Auth
{
    public record CustomUserClaims(string Name=null!, string Email =null!, int Role = 1, string UniqueId = null!);
    
}
