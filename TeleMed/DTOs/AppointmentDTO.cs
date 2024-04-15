using System.ComponentModel.DataAnnotations;

namespace TeleMed.DTOs;

public class AppointmentDto
{
    public int Id { get; set; }
    
    [Required]
    public int PatientId { get; set; }

    public string PatientName { get; set; } = string.Empty;
    
    [Required]
    public int ProviderId { get; set; }
    public string ProviderName { get; set; } = string.Empty;
    
    [Required]
    [DataType(DataType.Date)]
    public DateTime AppointmentDate { get; set; }

    [Required]
    public string AppointmentTime { get; set; } = string.Empty;
    
    [Required] 
    public string AppointmentStatus { get; set; } = string.Empty;
}