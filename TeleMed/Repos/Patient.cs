using TeleMed.Data.Abstracts;
using TeleMed.DTOs;
using TeleMed.DTOs.Auth;
using TeleMed.DTOs.Patient;
using TeleMed.Models;
using TeleMed.Repos.Abstracts;
using TeleMed.Responses;
using TeleMed.States;
using Exception = System.Exception;

namespace TeleMed.Repos;

public class Patient(IAccount accountRepo, IAppDbContext appDbContext)
    : IPatient
{
    public CustomResponses.PatientResponse CreatePatient(PatientRegisterDto patientDto)
    {
        try
        {
            if (patientDto == null!)
            {
                return new CustomResponses.PatientResponse
                {
                    Flag = false,
                    Message = "Patient data is required"
                };
            }
            
            var loginDto = new LoginDto
            {
                Email = patientDto.Email,
                Password = patientDto.Password
            };

            var findUser = accountRepo.GetUser(loginDto);
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (findUser is not null)
                return (new CustomResponses.PatientResponse(false, "User already exist"));

            //Create User in the database
            var registerResponse = accountRepo.RegisterAsync(new RegisterDto
            {
                Email = patientDto.Email,
                Password = patientDto.Password!,
                Name = patientDto.FirstName,
                Role = (int)UserRoles.Patient
            });

            if (!registerResponse.Item1.Flag || registerResponse.Item2 == 0)
            {
                return new CustomResponses.PatientResponse
                {
                    Flag = false,
                    Message = "Unable to create patient"
                };
            }

            //Create Patient in the database
            var newPatient = new Patients
            {
                UserId = registerResponse.Item2,
                FirstName = patientDto.FirstName,
                MiddleName = patientDto.MiddleName,
                LastName = patientDto.LastName,
                Gender = (int)patientDto.Gender,
                Email = patientDto.Email,
                Phone = patientDto.Phone,
                Address1 = patientDto.Address1,
                Address2 = patientDto.Address2,
                City = patientDto.City,
                State = patientDto.State,
                ZipCode = patientDto.ZipCode,
                Dob = patientDto.Dob.Date.ToUniversalTime()
            };

            appDbContext.Patients.Add(newPatient);
            appDbContext.SaveChanges();

            return new CustomResponses.PatientResponse
            {
                Flag = true,
                Message = "Patient created successfully"
            };

        }
        catch (Exception e)
        {
            //Log error
            return new CustomResponses.PatientResponse
            {
                Flag = false,
                Message = "An error occurred"
            };
        }
    }

    public CustomResponses.PatientResponse UpdatePatient(PatientDto patientDto)
    {
        try
        {
            var patient = GetPatient(patientDto.Id);
            if (patient.Id == 0)
            {
                return new CustomResponses.PatientResponse
                {
                    Flag = false,
                    Message = "Patient not found"
                };
            }
        
            patient.FirstName = patientDto.FirstName;
            patient.MiddleName = patientDto.MiddleName;
            patient.LastName = patientDto.LastName;
            patient.Email = patientDto.Email;
            patient.Phone = patientDto.Phone;
            patient.Address1 = patientDto.Address1;
            patient.Address2 = patientDto.Address2;
            patient.City = patientDto.City;
            patient.State = patientDto.State;
            patient.ZipCode = patientDto.ZipCode;
            patient.Dob = patientDto.Dob;
        
            appDbContext.Patients.Update(patient);
            appDbContext.SaveChanges();
        
            return new CustomResponses.PatientResponse
            {
                Flag = true,
                Message = "Patient updated successfully"
            };
        }
        catch (Exception e)
        {
            //Log error
            
            return new CustomResponses.PatientResponse
            {
                Flag = false,
                Message = "An error occurred"
            };
        }
        
    }

    public CustomResponses.PatientResponse DeletePatient(int patientId)
    {
        try
        {
            var patient = GetPatient(patientId);
            if (patient.Id == 0)
            {
                return new CustomResponses.PatientResponse
                {
                    Flag = false,
                    Message = "Patient not found"
                };
            }

            patient.Status = false;
            
            appDbContext.Patients.Update(patient);
            appDbContext.SaveChanges();

            return new CustomResponses.PatientResponse
            {
                Flag = true,
                Message = "Patient deleted successfully"
            };
        }
        catch (Exception e)
        {
            //Log error
            return new CustomResponses.PatientResponse
            {
                Flag = false,
                Message = "An error occurred"
            };
        }
    }

    public Patients GetPatient(int patientId)
    {
        var patient = appDbContext.Patients.FirstOrDefault(x => x.Id == patientId);
        return patient ?? new Patients();
    }

    public List<Patients> GetPatients()
    {
        return appDbContext.Patients.ToList();
    }
}