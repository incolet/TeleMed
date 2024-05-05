using Microsoft.EntityFrameworkCore;
using Moq;
using TeleMed.Data.Abstracts;
using TeleMed.DTOs.Auth;
using TeleMed.DTOs.Patient;
using TeleMed.Models;
using TeleMed.Repos;
using TeleMed.Repos.Abstracts;
using TeleMed.Responses;
using TeleMed.States;

namespace TeleMedicineTest;

public class PatientsTest
{
    private readonly Mock<IAppDbContext> _appDbContext;
    private readonly Patient _patientRepo;
    private readonly Mock<IAccount> _accountRepo;
    
    public PatientsTest()
    {
        _appDbContext = new Mock<IAppDbContext>();
        _accountRepo = new Mock<IAccount>();
        _patientRepo = new Patient(_accountRepo.Object,_appDbContext.Object);
    }
    
    [Fact]
    public void CreatePatient_ReturnsFailureResponse_WhenPatientDataIsNull()
    {
        // Arrange
        PatientRegisterDto patientDto = null!;

        // Act
        var result = _patientRepo.CreatePatient(patientDto);

        // Assert
        Assert.False(result.Flag);
        Assert.Equal("Patient data is required", result.Message);
    }

    [Fact]
    public void CreatePatient_ReturnsFailureResponse_WhenUserAlreadyExists()
    {
        // Arrange
        var patientDto = new PatientRegisterDto { Email = "test@test.com" };
        var loginDto = new LoginDto() { Email = patientDto.Email,Role = (int)UserRoles.Patient};
        _accountRepo.Setup(a => a.GetUser(loginDto)).Returns(new ApplicationUser() { Id = 2,Role = (int)UserRoles.Patient});

        // Act
        var result = _patientRepo.CreatePatient(patientDto);

        // Assert
        Assert.False(result.Flag);
        Assert.Equal("User already exist", result.Message);
    }

    [Fact]
    public void CreatePatient_ReturnsFailureResponse_WhenUnableToCreatePatient()
    {
        // Arrange
        var patientDto = new PatientRegisterDto { Email = "test@test.com", LastName = "Test" };
        var loginDto = new LoginDto() { Email = patientDto.Email};
        _accountRepo.Setup(a => a.GetUser(loginDto)).Returns(new ApplicationUser { Id = 0 });
        _accountRepo.Setup(a=>a.IsUserInRole(It.IsAny<ApplicationUser>(),It.IsAny<int>())).Returns(true);
        _accountRepo.Setup(a => a.RegisterAsync(It.IsAny<RegisterDto>())).Returns((new CustomResponses.RegistrationResponse(false, "Unable to register"), 0));

        // Act
        var result = _patientRepo.CreatePatient(patientDto);

        // Assert
        Assert.False(result.Flag);
        Assert.Equal("Unable to create patient", result.Message);
    }
    
    [Fact]
    public void CreatePatient_ReturnsSuccessResponse_WhenPatientCreatedSuccessfully()
    {
        // Arrange
        var patientDto = new PatientRegisterDto() { Email = "test@test.com", LastName = "Test" };
        var loginDto = new LoginDto() { Email = patientDto.Email};
        _accountRepo.Setup(a => a.GetUser(loginDto)).Returns(new ApplicationUser() { Id = 0 });
        _accountRepo.Setup(a=>a.IsUserInRole(It.IsAny<ApplicationUser>(),It.IsAny<int>())).Returns(true);
        _accountRepo.Setup(a => a.RegisterAsync(It.IsAny<RegisterDto>())).Returns((new CustomResponses.RegistrationResponse(true, "Registered successfully"), 1));

        // Mocking the Patients DbSet
        // ReSharper disable once CollectionNeverUpdated.Local
        var patients = new List<Patients>();
        var mockDbSetPatients = new Mock<DbSet<Patients>>();
        mockDbSetPatients.As<IQueryable<Patients>>().Setup(m => m.Provider).Returns(patients.AsQueryable().Provider);
        mockDbSetPatients.As<IQueryable<Patients>>().Setup(m => m.Expression).Returns(patients.AsQueryable().Expression);
        mockDbSetPatients.As<IQueryable<Patients>>().Setup(m => m.ElementType).Returns(patients.AsQueryable().ElementType);
        using var enumeratorPatients = patients.AsQueryable().GetEnumerator();
        mockDbSetPatients.As<IQueryable<Patients>>().Setup(m => m.GetEnumerator()).Returns(enumeratorPatients);
        _appDbContext.Setup(db => db.Patients).Returns(mockDbSetPatients.Object);

        // Act
        var result = _patientRepo.CreatePatient(patientDto);

        // Assert
        Assert.True(result.Flag);
        Assert.Equal("Patient created successfully", result.Message);
    }
    
  
    
    
}