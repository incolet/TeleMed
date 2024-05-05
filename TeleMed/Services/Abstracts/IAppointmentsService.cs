using TeleMed.DTOs.Appointment;
using TeleMed.DTOs.Provider;
using TeleMed.Responses;

namespace TeleMed.Services.Abstracts;

public interface IAppointmentsService
{
    Task<List<AppointmentDto>> GetAppointmentsByPatientId(int patientId);
    Task<List<AppointmentDto>> GetAppointmentsByProviderId(int providerId);
    Task<CustomResponses.AppointmentResponse> CreateAppointment(AppointmentDto appointmentDto);
    
    Task<List<DateTime>> GetAvailableAppointmentTimes(int providerId, string appointmentDate);
}