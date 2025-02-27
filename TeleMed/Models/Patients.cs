using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeleMed.Models;

public class Patients
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    public int UserId { get; set; }
    
    [Required]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;
    
    [StringLength(50)]
    public string? MiddleName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string Address1 { get; set; } = string.Empty;
    
    [StringLength(50)]
    public string? Address2 { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string City { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string State { get; set; } = string.Empty;
    
    [Required]
    [StringLength(10)]
    public string ZipCode { get; set; } = string.Empty;

    public int Gender { get; set; }
    
    [Required]
    [DataType(DataType.Date)]
    public DateTime Dob { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [StringLength(20)]
    public string Phone { get; set; } = string.Empty;

    public bool Status { get; set; }
}