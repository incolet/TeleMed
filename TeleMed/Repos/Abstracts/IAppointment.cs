using TeleMed.DTOs;
using static TeleMed.Responses.CustomResponses;

namespace TeleMed.Repos.Abstracts;

public interface IAppointment
{
    AppointmentResponse CreateAppointment(AppointmentDto appointmentDto);
    AppointmentResponse UpdateAppointment(AppointmentDto appointmentDto);
    AppointmentResponse DeleteAppointment(int appointmentId);
    AppointmentDto GetAppointment(int appointmentId);
    List<AppointmentDto> GetAppointments();
    
    List<AppointmentDto> GetAppointmentsByPatient(int patientId);
    
    List<AppointmentDto> GetAppointmentsByProvider(int providerId);
    
    AppointmentResponse CancelAppointment(int appointmentId);
    
}