using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TeleMed.Models
{
    public class ApplicationUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
        
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [StringLength(50)]
        public string Email { get; set; } = string.Empty;
        public int Role { get; set; } = 1;
        
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; } = string.Empty;
    }
}
