using TeleMed.Components.Pages;
using TeleMed.DTOs.Patient;
using TeleMed.Responses;

namespace TeleMed.Services.Abstracts;

public interface IPatientsService
{
    Task<CustomResponses.PatientResponse> CreatePatient(PatientRegisterDto patientDto);
}