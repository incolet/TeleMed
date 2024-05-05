using System.ComponentModel.DataAnnotations;
using TeleMed.States;

namespace TeleMed.DTOs.Patient;

public class PatientDto
{
    public int Id { get; set; }
    
    [Required]
    public int UserId { get; set; }
    
    [Required]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;
    
    [StringLength(50)]
    public string MiddleName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string Address1 { get; set; } = string.Empty;
    
    [StringLength(50)]
    public string Address2 { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string City { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string State { get; set; } = string.Empty;
    
    [Required]
    [StringLength(10)]
    public string ZipCode { get; set; } = string.Empty;
    
    [Required]
    [DataType(DataType.Date)]
    public DateTime Dob { get; set; } = DateTime.Now;
    
    [Required]
    [StringLength(50)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [StringLength(20)]
    [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid phone number")]
    public string Phone { get; set; } = string.Empty;

    [Required]
    public Gender Gender { get; set; }
}