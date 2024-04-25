using Microsoft.AspNetCore.Mvc;
using TeleMed.DTOs;
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
        public PatientResponse CreatePatient([FromBody] PatientDto patientDto)
        {
            return patientRepo.CreatePatient(patientDto);
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