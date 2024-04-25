using TeleMed.DTOs.Appointment;
using TeleMed.DTOs.Provider;
using TeleMed.Responses;

namespace TeleMed.Services.Abstracts;

public interface IAppointmentsService
{
    Task<List<AppointmentDto>> GetAppointmentsByPatientId(int patientId);
    Task<CustomResponses.AppointmentResponse> CreateAppointment(AppointmentDto appointmentDto);
    
    Task<List<DateTime>> GetAvailableAppointmentTimes(int providerId, DateTime appointmentDate);
}