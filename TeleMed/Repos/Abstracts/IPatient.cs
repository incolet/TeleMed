using TeleMed.DTOs;
using TeleMed.DTOs.Patient;
using TeleMed.Models;
using static TeleMed.Responses.CustomResponses;

namespace TeleMed.Repos.Abstracts;

public interface IPatient
{
    PatientResponse CreatePatient(PatientRegisterDto patientDto);
    PatientResponse UpdatePatient(PatientDto patientDto);
    PatientResponse DeletePatient(int patientId);
    Patients GetPatient(int patientId);
    List<Patients> GetPatients();
}