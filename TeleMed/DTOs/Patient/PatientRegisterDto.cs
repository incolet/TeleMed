using System.ComponentModel.DataAnnotations;
namespace TeleMed.DTOs.Patient;

public class PatientRegisterDto : PatientDto
{ 
    [Required]
[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
[DataType(DataType.Password)]
[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,100}$", ErrorMessage = "The password must have at least one non-alphanumeric character, one digit ('0'-'9'), one uppercase ('A'-'Z').")]
public string Password { get; set; } = string.Empty;
    
}