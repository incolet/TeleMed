using System.ComponentModel.DataAnnotations;

namespace TeleMed.DTOs.Auth
{
    public class RegisterDto : LoginDTO
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required, Compare(nameof(Password)), DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
