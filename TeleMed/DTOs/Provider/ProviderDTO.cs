using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeleMed.DTOs.Provider;

public class ProviderDto
{
    
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    public int UserId { get; set; }
    
    [Required]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string Email { get; set; } = string.Empty;
    
    [StringLength(20)]
    public string Phone { get; set; } = string.Empty;
}