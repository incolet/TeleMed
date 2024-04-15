using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeleMed.Models;

public class Appointments
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    public int PatientId { get; set; }
    
    [Required]
    public int ProviderId { get; set; }
    
    [Required]
    [DataType(DataType.Date)]
    public DateTime AppointmentDate { get; set; }

    [Required]
    [StringLength(10)]
    public string AppointmentTime { get; set; } = string.Empty;
    
    [Required] 
    public int AppointmentStatus { get; set; }

    public bool Status { get; set; }
}