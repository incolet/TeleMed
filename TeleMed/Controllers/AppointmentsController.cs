using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeleMed.DTOs.Appointment;
using TeleMed.Repos.Abstracts;
using static TeleMed.Responses.CustomResponses;

namespace TeleMed.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController(IAppointment appointmentRepo) : ControllerBase
    {
        [HttpPost]
        public IActionResult CreateAppointment([FromBody] AppointmentDto appointmentDto)
        {
            var result = appointmentRepo.CreateAppointment(appointmentDto);
            return Ok(result);
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

        [HttpGet("patient/{patientId:int}")]
        public IActionResult GetAppointmentsByPatient([FromRoute] int patientId)
        {
            var result = appointmentRepo.GetAppointmentsByPatient(patientId);
            
            return Ok(result);
        }

        [HttpGet("provider/{providerId}")]
        public IActionResult GetAppointmentsByProvider(int providerId)
        {
            var result = appointmentRepo.GetAppointmentsByProvider(providerId);
            return Ok(result);
        }

        [HttpPut("cancel/{appointmentId}")]
        public AppointmentResponse CancelAppointment(int appointmentId)
        {
            return appointmentRepo.CancelAppointment(appointmentId);
        }
        
        [HttpGet("time-slots/{providerId}/{date}")]
        public IActionResult GetTimeSlots([FromRoute] int providerId,string date)
        {
            var newDate = Convert.ToDateTime(date);
            var result = appointmentRepo.GetAvailableTimeSlots(providerId,newDate);
            return Ok(result);
        }
    }
}