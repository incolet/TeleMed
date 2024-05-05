using Microsoft.AspNetCore.Mvc;
using TeleMed.DTOs;
using TeleMed.DTOs.Patient;
using TeleMed.Models;
using TeleMed.Repos.Abstracts;
using static TeleMed.Responses.CustomResponses;

namespace TeleMed.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController(IPatient patientRepo) : ControllerBase
    {
        [HttpPost]
        public IActionResult CreatePatient([FromBody] PatientRegisterDto patientDto)
        {
            var result = patientRepo.CreatePatient(patientDto);
            return Ok(result);
        }

        [HttpPut]
        public PatientResponse UpdatePatient([FromBody] PatientDto patientDto)
        {
            return patientRepo.UpdatePatient(patientDto);
        }

        [HttpDelete("{patientId}")]
        public PatientResponse DeletePatient(int patientId)
        {
            return patientRepo.DeletePatient(patientId);
        }

        [HttpGet("{patientId}")]
        public Patients GetPatient(int patientId)
        {
            return patientRepo.GetPatient(patientId);
        }

        [HttpGet]
        public List<Patients> GetPatients()
        {
            return patientRepo.GetPatients();
        }
    }
}