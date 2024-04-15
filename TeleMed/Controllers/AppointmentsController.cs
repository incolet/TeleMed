using Microsoft.AspNetCore.Mvc;
using TeleMed.DTOs;
using TeleMed.Repos.Abstracts;
using static TeleMed.Responses.CustomResponses;

namespace TeleMed.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController(IAppointment appointmentRepo) : ControllerBase
    {
        [HttpPost]
        public AppointmentResponse CreateAppointment([FromBody] AppointmentDto appointmentDto)
        {
            return appointmentRepo.CreateAppointment(appointmentDto);
        }

        [HttpPut]
        public AppointmentResponse UpdateAppointment([FromBody] AppointmentDto appointmentDto)
        {
            return appointmentRepo.UpdateAppointment(appointmentDto);
        }

        [HttpDelete("{appointmentId}")]
        public AppointmentResponse DeleteAppointment(int appointmentId)
        {
            return appointmentRepo.DeleteAppointment(appointmentId);
        }

        [HttpGet("{appointmentId}")]
        public AppointmentDto GetAppointment(int appointmentId)
        {
            return appointmentRepo.GetAppointment(appointmentId);
        }

        [HttpGet]
        public List<AppointmentDto> GetAppointments()
        {
            return appointmentRepo.GetAppointments();
        }

        [HttpGet("patient/{patientId}")]
        public List<AppointmentDto> GetAppointmentsByPatient(int patientId)
        {
            return appointmentRepo.GetAppointmentsByPatient(patientId);
        }

        [HttpGet("provider/{providerId}")]
        public List<AppointmentDto> GetAppointmentsByProvider(int providerId)
        {
            return appointmentRepo.GetAppointmentsByProvider(providerId);
        }

        [HttpPut("cancel/{appointmentId}")]
        public AppointmentResponse CancelAppointment(int appointmentId)
        {
            return appointmentRepo.CancelAppointment(appointmentId);
        }
    }
}