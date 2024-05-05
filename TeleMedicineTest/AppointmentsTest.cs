using Microsoft.EntityFrameworkCore;
using Moq;
using TeleMed.Data.Abstracts;
using TeleMed.DTOs.Appointment;
using TeleMed.Models;
using TeleMed.Repos;
using TeleMed.Repos.Abstracts;
using TeleMed.States;

namespace TeleMedicineTest;

public class AppointmentsTest
{
    private readonly Mock<IAppDbContext> _appDbContext;
    private readonly IAppointment _appointmentRepo;

    public AppointmentsTest()
    {
        _appDbContext = new Mock<IAppDbContext>();
        _appointmentRepo = new Appointment(_appDbContext.Object);
    }

    [Fact]
    public void CreateAppointment_ReturnsFailure_WhenProviderDoesNotExist()
    {
        // Arrange
        var appointmentDto = new AppointmentDto { ProviderId = 1 };
        _appDbContext.Setup(db => db.Providers.Find(appointmentDto.ProviderId)).Returns((Providers)null!);

        // Act
        var result = _appointmentRepo.CreateAppointment(appointmentDto);

        // Assert
        Assert.False(result.Flag);
        Assert.Equal("Provider not found", result.Message);
    }

    [Fact]
    public void CreateAppointment_ReturnsFailure_WhenAppointmentTimeIsInThePast()
    {
        // Arrange
        var appointmentDto = new AppointmentDto { ProviderId = 1, AppointmentDate = DateTime.Now.AddDays(-1) };
        var provider = new Providers { Id = appointmentDto.ProviderId };
        _appDbContext.Setup(db => db.Providers.Find(appointmentDto.ProviderId)).Returns(provider);

        // Act
        var result = _appointmentRepo.CreateAppointment(appointmentDto);

        // Assert
        Assert.False(result.Flag);
        Assert.Equal("Appointment time must be in the future", result.Message);
    }

    [Fact]
    public void CreateAppointment_ReturnsFailure_WhenProviderAlreadyHasAnAppointmentAtTheSameTime()
    {
        // Arrange
        var appointmentDto = new AppointmentDto { ProviderId = 1, AppointmentDate = DateTime.Now.AddDays(1).ToUniversalTime().Date, AppointmentTime = "10:00 AM" };
        
        var provider = new Providers { Id = appointmentDto.ProviderId };
        _appDbContext.Setup(db => db.Providers.Find(appointmentDto.ProviderId)).Returns(provider);
        
        // Mocking the Appointments to return a list with some appointments
        var existingAppointment = new List<Appointments>
        {
            new Appointments { ProviderId = provider.Id, AppointmentDate = appointmentDto.AppointmentDate, AppointmentTime = appointmentDto.AppointmentTime }
        };
        
        var queryableAppointments = existingAppointment.AsQueryable();

        var mockDbSet = new Mock<DbSet<Appointments>>();
        mockDbSet.As<IQueryable<Appointments>>().Setup(m => m.Provider).Returns(queryableAppointments.Provider);
        mockDbSet.As<IQueryable<Appointments>>().Setup(m => m.Expression).Returns(queryableAppointments.Expression);
        mockDbSet.As<IQueryable<Appointments>>().Setup(m => m.ElementType).Returns(queryableAppointments.ElementType);
        using var enumerator = queryableAppointments.GetEnumerator();
        mockDbSet.As<IQueryable<Appointments>>().Setup(m => m.GetEnumerator()).Returns(enumerator);

        _appDbContext.Setup(db => db.Appointments).Returns(mockDbSet.Object);

        // Act
        var result = _appointmentRepo.CreateAppointment(appointmentDto);

        // Assert
        Assert.False(result.Flag);
        Assert.Equal("Provider already has an appointment at this time", result.Message);
    }

    [Fact]
    public void CreateAppointment_ReturnsSuccess_WhenAppointmentIsCreatedSuccessfully()
    {
        // Arrange
        var appointmentDto = new AppointmentDto { PatientId = 1,ProviderId = 1, AppointmentDate = DateTime.Now.AddDays(1).ToUniversalTime().Date, AppointmentTime = "10:00 AM" };
        
        var provider = new Providers { Id = appointmentDto.ProviderId };
        _appDbContext.Setup(db => db.Providers.Find(appointmentDto.ProviderId)).Returns(provider);
        
        var patients = new List<Patients>
        {
            new Patients { UserId = 1,Id = 1, FirstName = "Jane", LastName = "Doe" }
        };
        
        var mockDbSetPatients = new Mock<DbSet<Patients>>();
        mockDbSetPatients.As<IQueryable<Patients>>().Setup(m => m.Provider).Returns(patients.AsQueryable().Provider);
        mockDbSetPatients.As<IQueryable<Patients>>().Setup(m => m.Expression).Returns(patients.AsQueryable().Expression);
        mockDbSetPatients.As<IQueryable<Patients>>().Setup(m => m.ElementType).Returns(patients.AsQueryable().ElementType);
        using var value = patients.AsQueryable().GetEnumerator();
        mockDbSetPatients.As<IQueryable<Patients>>().Setup(m => m.GetEnumerator()).Returns(value);
        _appDbContext.Setup(db => db.Patients).Returns(mockDbSetPatients.Object);

        var appointments = new List<Appointments>
        {
            new Appointments { ProviderId = 2, AppointmentDate = DateTime.Now.AddDays(1), AppointmentTime = "10:00 AM",Status = true},
            
        };

        var queryableAppointments = appointments.AsQueryable();

        var mockDbSet = new Mock<DbSet<Appointments>>();
        mockDbSet.As<IQueryable<Appointments>>().Setup(m => m.Provider).Returns(queryableAppointments.Provider);
        mockDbSet.As<IQueryable<Appointments>>().Setup(m => m.Expression).Returns(queryableAppointments.Expression);
        mockDbSet.As<IQueryable<Appointments>>().Setup(m => m.ElementType).Returns(queryableAppointments.ElementType);
        using var enumerator = queryableAppointments.GetEnumerator();
        mockDbSet.As<IQueryable<Appointments>>().Setup(m => m.GetEnumerator()).Returns(enumerator);

        _appDbContext.Setup(db => db.Appointments).Returns(mockDbSet.Object);

        // Act
        var result = _appointmentRepo.CreateAppointment(appointmentDto);

        // Assert
        Assert.True(result.Flag);
        Assert.Equal("Appointment created successfully", result.Message);
    }
    
    
    [Fact]
    public void GetAvailableTimeSlots_ReturnsEmptyList_WhenProviderDoesNotExist()
    {
        // Arrange
        const int nonExistentProviderId = 999;
        var date = DateTime.Now;

        // Mocking the Providers to return null
        _appDbContext.Setup(db => db.Providers.Find(nonExistentProviderId)).Returns((Providers)null!);

        // Act
        var result = _appointmentRepo.GetAvailableTimeSlots(nonExistentProviderId, date);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetAvailableTimeSlots_ReturnsAllTimeSlots_WhenNoAppointmentsExist()
    {
        // Arrange
        const int providerId = 1;
        var date = DateTime.Now;

        // Mocking the Provider to return a valid provider
        var provider = new Providers { Id = providerId };
        _appDbContext.Setup(db => db.Providers.Find(providerId)).Returns(provider);

        // Mocking the Appointments to return an empty list
        // ReSharper disable once CollectionNeverUpdated.Local
        var appointments = new List<Appointments>();
        var dbSetMock = new Mock<DbSet<Appointments>>();
        dbSetMock.As<IQueryable<Appointments>>().Setup(m => m.Provider).Returns(appointments.AsQueryable().Provider);
        dbSetMock.As<IQueryable<Appointments>>().Setup(m => m.Expression).Returns(appointments.AsQueryable().Expression);
        dbSetMock.As<IQueryable<Appointments>>().Setup(m => m.ElementType).Returns(appointments.AsQueryable().ElementType);
        using var enumerator = new List<Appointments>().AsQueryable().GetEnumerator();
        dbSetMock.As<IQueryable<Appointments>>().Setup(m => m.GetEnumerator()).Returns(enumerator);
        _appDbContext.Setup(db => db.Appointments).Returns(dbSetMock.Object);

        // Act
        var result = _appointmentRepo.GetAvailableTimeSlots(providerId, date);

        // Assert
        Assert.Equal((AppointmentConstants.EndTime - AppointmentConstants.StartTime) * (60 / AppointmentConstants.AppointmentDuration), result.Count);
    }

    [Fact]
    public void GetAvailableTimeSlots_ReturnsRemainingTimeSlots_WhenSomeAppointmentsExist()
    {
        // Arrange
        const int providerId = 1;
        var date = DateTime.Now.ToUniversalTime().Date;
        
        // Mocking the Provider to return a valid provider
        var provider = new Providers { Id = providerId };
        _appDbContext.Setup(db => db.Providers.Find(providerId)).Returns(provider);

        // Mocking the Appointments to return a list with some appointments
        var appointments = new List<Appointments>
        {
            new Appointments { ProviderId = providerId, AppointmentDate = date, AppointmentTime = "10:00 AM",Status = true},
            new Appointments { ProviderId = providerId, AppointmentDate = date, AppointmentTime = "11:00 AM",Status = true}
        };
        var dbSetMock = new Mock<DbSet<Appointments>>();
        dbSetMock.As<IQueryable<Appointments>>().Setup(m => m.Provider).Returns(appointments.AsQueryable().Provider);
        dbSetMock.As<IQueryable<Appointments>>().Setup(m => m.Expression).Returns(appointments.AsQueryable().Expression);
        dbSetMock.As<IQueryable<Appointments>>().Setup(m => m.ElementType).Returns(appointments.AsQueryable().ElementType);
        using var enumerator = appointments.AsQueryable().GetEnumerator();
        dbSetMock.As<IQueryable<Appointments>>().Setup(m => m.GetEnumerator()).Returns(enumerator);
        _appDbContext.Setup(db => db.Appointments).Returns(dbSetMock.Object);

        // Act
        var result = _appointmentRepo.GetAvailableTimeSlots(providerId, date);

        // Assert
        Assert.Equal((AppointmentConstants.EndTime - AppointmentConstants.StartTime) * (60 / AppointmentConstants.AppointmentDuration) - appointments.Count, result.Count);
        Assert.DoesNotContain(result, timeSlot => appointments.Any(a => a.AppointmentTime == timeSlot.ToString("HH:mm")));
    }
    
    [Fact]
    public void CancelAppointment_ReturnsFailure_WhenAppointmentDoesNotExist()
    {
        // Arrange
        const int nonExistentAppointmentId = 999;
        
        // Mocking the Appointments to return null
        _appDbContext.Setup(db => db.Appointments.Find(nonExistentAppointmentId)).Returns((Appointments)null!);

        // Act
        var result = _appointmentRepo.CancelAppointment(nonExistentAppointmentId);

        // Assert
        Assert.False(result.Flag);
        Assert.Equal("Appointment not found", result.Message);
    }
    
    [Fact]
    public void CancelAppointment_ReturnsSuccess_WhenAppointmentIsCancelledSuccessfully()
    {
        // Arrange
        const int appointmentId = 1;
        var appointment = new Appointments { Id = appointmentId, AppointmentStatus = (int)AppointmentStatus.Scheduled };
        _appDbContext.Setup(db => db.Appointments.Find(appointmentId)).Returns(appointment);

        // Act
        var result = _appointmentRepo.CancelAppointment(appointmentId);

        // Assert
        Assert.True(result.Flag);
        Assert.Equal("Appointment cancelled successfully", result.Message);
        Assert.Equal(AppointmentStatus.Cancelled, (AppointmentStatus)appointment.AppointmentStatus);
    }
    
    [Fact]
    public void RescheduleAppointment_ReturnsFailure_WhenAppointmentDoesNotExist()
    {
        // Arrange
        const int nonExistentAppointmentId = 999;
        
        // Mocking the Appointments to return null
        _appDbContext.Setup(db => db.Appointments.Find(nonExistentAppointmentId)).Returns((Appointments)null!);

        // Act
        var result = _appointmentRepo.RescheduleAppointment(nonExistentAppointmentId, DateTime.Now, "10:00");

        // Assert
        Assert.False(result.Flag);
        Assert.Equal("Appointment not found", result.Message);
    }
    
    [Fact]
    public void RescheduleAppointment_ReturnsSuccess_WhenAppointmentIsRescheduledSuccessfully()
    {
        // Arrange
        const int appointmentId = 1;
        var appointment = new Appointments { Id = appointmentId, AppointmentStatus = (int)AppointmentStatus.Scheduled };
        _appDbContext.Setup(db => db.Appointments.Find(appointmentId)).Returns(appointment);

        var newDate = DateTime.Now.AddDays(1);
        const string newTime = "10:00";

        // Act
        var result = _appointmentRepo.RescheduleAppointment(appointmentId, newDate, newTime);

        // Assert
        Assert.True(result.Flag);
        Assert.Equal("Appointment rescheduled successfully", result.Message);
        Assert.Equal(newDate, appointment.AppointmentDate);
        Assert.Equal(newTime, appointment.AppointmentTime);
    }
    
    [Fact]
    public void GetAppointmentsByProvider_ReturnsAppointments_WhenProviderHasAppointments()
    {
        // Arrange
        const int providerId = 1;
        
        var providers = new List<Providers>
        {
            new Providers { UserId = 1,Id = 1, FirstName = "John", LastName = "Doe" }
        };

        var queryableProviders = providers.AsQueryable();

        var mockDbSetProviders = new Mock<DbSet<Providers>>();
        mockDbSetProviders.As<IQueryable<Providers>>().Setup(m => m.Provider).Returns(queryableProviders.Provider);
        mockDbSetProviders.As<IQueryable<Providers>>().Setup(m => m.Expression).Returns(queryableProviders.Expression);
        mockDbSetProviders.As<IQueryable<Providers>>().Setup(m => m.ElementType).Returns(queryableProviders.ElementType);
        using var value = queryableProviders.GetEnumerator();
        mockDbSetProviders.As<IQueryable<Providers>>().Setup(m => m.GetEnumerator()).Returns(value);

        _appDbContext.Setup(db => db.Providers).Returns(mockDbSetProviders.Object);

        // Mocking the Patients DbSet
        var patients = new List<Patients>
        {
            new Patients { UserId = 1,Id = 1, FirstName = "Jane", LastName = "Doe" }
        };

        var queryablePatients = patients.AsQueryable();

        var mockDbSetPatients = new Mock<DbSet<Patients>>();
        mockDbSetPatients.As<IQueryable<Patients>>().Setup(m => m.Provider).Returns(queryablePatients.Provider);
        mockDbSetPatients.As<IQueryable<Patients>>().Setup(m => m.Expression).Returns(queryablePatients.Expression);
        mockDbSetPatients.As<IQueryable<Patients>>().Setup(m => m.ElementType).Returns(queryablePatients.ElementType);
        using var patientEnumerator = queryablePatients.GetEnumerator();
        mockDbSetPatients.As<IQueryable<Patients>>().Setup(m => m.GetEnumerator()).Returns(patientEnumerator);

        _appDbContext.Setup(db => db.Patients).Returns(mockDbSetPatients.Object);
        
        // Mocking the Appointments to return a list with some appointments
        var appointments = new List<Appointments>
        {
            new Appointments { ProviderId = providerId, PatientId = 1, AppointmentDate = DateTime.Now, AppointmentTime = "10:00", Status = true},
            new Appointments { ProviderId = providerId, PatientId = 1, AppointmentDate = DateTime.Now, AppointmentTime = "11:00", Status = true}
        };
        var dbSetMock = new Mock<DbSet<Appointments>>();
        dbSetMock.As<IQueryable<Appointments>>().Setup(m => m.Provider).Returns(appointments.AsQueryable().Provider);
        dbSetMock.As<IQueryable<Appointments>>().Setup(m => m.Expression).Returns(appointments.AsQueryable().Expression);
        dbSetMock.As<IQueryable<Appointments>>().Setup(m => m.ElementType).Returns(appointments.AsQueryable().ElementType);
        using var enumerator = appointments.AsQueryable().GetEnumerator();
        dbSetMock.As<IQueryable<Appointments>>().Setup(m => m.GetEnumerator()).Returns(enumerator);
        _appDbContext.Setup(db => db.Appointments).Returns(dbSetMock.Object);

        // Act
        var result = _appointmentRepo.GetAppointmentsByProvider(providerId);

        // Assert
        Assert.Equal(appointments.Count, result.Count);
        Assert.All(result, appointment => Assert.Equal(providerId, appointment.ProviderId));
    }
    
    [Fact]
    public void GetAppointmentsByProvider_ReturnsEmptyList_WhenProviderHasNoAppointments()
    {
        // Arrange
        const int providerId = 1;

        // Mocking the Providers DbSet
        var providers = new List<Providers>
        {
            new Providers { Id = providerId, FirstName = "John", LastName = "Doe" }
        };
        var mockDbSetProviders = new Mock<DbSet<Providers>>();
        mockDbSetProviders.As<IQueryable<Providers>>().Setup(m => m.Provider).Returns(providers.AsQueryable().Provider);
        mockDbSetProviders.As<IQueryable<Providers>>().Setup(m => m.Expression).Returns(providers.AsQueryable().Expression);
        mockDbSetProviders.As<IQueryable<Providers>>().Setup(m => m.ElementType).Returns(providers.AsQueryable().ElementType);
        using var enumerator = providers.AsQueryable().GetEnumerator();
        mockDbSetProviders.As<IQueryable<Providers>>().Setup(m => m.GetEnumerator()).Returns(enumerator);
        _appDbContext.Setup(db => db.Providers).Returns(mockDbSetProviders.Object);

        // Mocking the Patients DbSet
        var patients = new List<Patients>
        {
            new Patients {UserId = 1,Id = 1, FirstName = "Jane", LastName = "Doe" }
        };
        var mockDbSetPatients = new Mock<DbSet<Patients>>();
        mockDbSetPatients.As<IQueryable<Patients>>().Setup(m => m.Provider).Returns(patients.AsQueryable().Provider);
        mockDbSetPatients.As<IQueryable<Patients>>().Setup(m => m.Expression).Returns(patients.AsQueryable().Expression);
        mockDbSetPatients.As<IQueryable<Patients>>().Setup(m => m.ElementType).Returns(patients.AsQueryable().ElementType);
        using var value = patients.AsQueryable().GetEnumerator();
        mockDbSetPatients.As<IQueryable<Patients>>().Setup(m => m.GetEnumerator()).Returns(value);
        _appDbContext.Setup(db => db.Patients).Returns(mockDbSetPatients.Object);

        // Mocking the Appointments DbSet to return an empty list
        // ReSharper disable once CollectionNeverUpdated.Local
        var appointments = new List<Appointments>();
        var mockDbSetAppointments = new Mock<DbSet<Appointments>>();
        mockDbSetAppointments.As<IQueryable<Appointments>>().Setup(m => m.Provider).Returns(appointments.AsQueryable().Provider);
        mockDbSetAppointments.As<IQueryable<Appointments>>().Setup(m => m.Expression).Returns(appointments.AsQueryable().Expression);
        mockDbSetAppointments.As<IQueryable<Appointments>>().Setup(m => m.ElementType).Returns(appointments.AsQueryable().ElementType);
        using var enumerator1 = appointments.AsQueryable().GetEnumerator();
        mockDbSetAppointments.As<IQueryable<Appointments>>().Setup(m => m.GetEnumerator()).Returns(enumerator1);
        _appDbContext.Setup(db => db.Appointments).Returns(mockDbSetAppointments.Object);

        // Act
        var result = _appointmentRepo.GetAppointmentsByProvider(providerId);

        // Assert
        Assert.Empty(result);
    }    
    [Fact]
    public void GetAppointmentsByPatient_ReturnsAppointments_WhenPatientHasAppointments()
    {
        // Arrange
        const int patientId = 1;
        const int providerId = 1;

        // Mocking the Providers DbSet
        var providers = new List<Providers>
        {
            new Providers { Id = providerId, FirstName = "John", LastName = "Doe" }
        };
        var mockDbSetProviders = new Mock<DbSet<Providers>>();
        mockDbSetProviders.As<IQueryable<Providers>>().Setup(m => m.Provider).Returns(providers.AsQueryable().Provider);
        mockDbSetProviders.As<IQueryable<Providers>>().Setup(m => m.Expression).Returns(providers.AsQueryable().Expression);
        mockDbSetProviders.As<IQueryable<Providers>>().Setup(m => m.ElementType).Returns(providers.AsQueryable().ElementType);
        using var enumerator = providers.AsQueryable().GetEnumerator();
        mockDbSetProviders.As<IQueryable<Providers>>().Setup(m => m.GetEnumerator()).Returns(enumerator);
        _appDbContext.Setup(db => db.Providers).Returns(mockDbSetProviders.Object);

        // Mocking the Patients DbSet
        var patients = new List<Patients>
        {
            new Patients { UserId = 1,Id = patientId, FirstName = "Jane", LastName = "Doe" }
        };
        var mockDbSetPatients = new Mock<DbSet<Patients>>();
        mockDbSetPatients.As<IQueryable<Patients>>().Setup(m => m.Provider).Returns(patients.AsQueryable().Provider);
        mockDbSetPatients.As<IQueryable<Patients>>().Setup(m => m.Expression).Returns(patients.AsQueryable().Expression);
        mockDbSetPatients.As<IQueryable<Patients>>().Setup(m => m.ElementType).Returns(patients.AsQueryable().ElementType);
        using var value = patients.AsQueryable().GetEnumerator();
        mockDbSetPatients.As<IQueryable<Patients>>().Setup(m => m.GetEnumerator()).Returns(value);
        _appDbContext.Setup(db => db.Patients).Returns(mockDbSetPatients.Object);

        // Mocking the Appointments DbSet
        var appointments = new List<Appointments>
        {
            new Appointments { PatientId = patientId, ProviderId = providerId, AppointmentDate = DateTime.Now, AppointmentTime = "10:00",Status = true},
            new Appointments { PatientId = patientId, ProviderId = providerId, AppointmentDate = DateTime.Now, AppointmentTime = "11:00", Status = true}
        };
        var mockDbSetAppointments = new Mock<DbSet<Appointments>>();
        mockDbSetAppointments.As<IQueryable<Appointments>>().Setup(m => m.Provider).Returns(appointments.AsQueryable().Provider);
        mockDbSetAppointments.As<IQueryable<Appointments>>().Setup(m => m.Expression).Returns(appointments.AsQueryable().Expression);
        mockDbSetAppointments.As<IQueryable<Appointments>>().Setup(m => m.ElementType).Returns(appointments.AsQueryable().ElementType);
        using var enumerator1 = appointments.AsQueryable().GetEnumerator();
        mockDbSetAppointments.As<IQueryable<Appointments>>().Setup(m => m.GetEnumerator()).Returns(enumerator1);
        _appDbContext.Setup(db => db.Appointments).Returns(mockDbSetAppointments.Object);

        // Act
        var result = _appointmentRepo.GetAppointmentsByPatient(patientId);

        // Assert
        Assert.Equal(appointments.Count, result.Count);
        Assert.All(result, appointment => Assert.Equal(patientId, appointment.PatientId));
    }
    
    [Fact]
    public void GetAppointmentsByPatient_ReturnsEmptyList_WhenPatientHasNoAppointments()
    {
        // Arrange
        const int patientId = 1;

        // Mocking the Providers DbSet
        var providers = new List<Providers>
        {
            new Providers { Id = 1, FirstName = "John", LastName = "Doe" }
        };
        var mockDbSetProviders = new Mock<DbSet<Providers>>();
        mockDbSetProviders.As<IQueryable<Providers>>().Setup(m => m.Provider).Returns(providers.AsQueryable().Provider);
        mockDbSetProviders.As<IQueryable<Providers>>().Setup(m => m.Expression).Returns(providers.AsQueryable().Expression);
        mockDbSetProviders.As<IQueryable<Providers>>().Setup(m => m.ElementType).Returns(providers.AsQueryable().ElementType);
        using var enumerator = providers.AsQueryable().GetEnumerator();
        mockDbSetProviders.As<IQueryable<Providers>>().Setup(m => m.GetEnumerator()).Returns(enumerator);
        _appDbContext.Setup(db => db.Providers).Returns(mockDbSetProviders.Object);

        // Mocking the Patients DbSet
        var patients = new List<Patients>
        {
            new Patients { Id = patientId, FirstName = "Jane", LastName = "Doe" }
        };
        var mockDbSetPatients = new Mock<DbSet<Patients>>();
        mockDbSetPatients.As<IQueryable<Patients>>().Setup(m => m.Provider).Returns(patients.AsQueryable().Provider);
        mockDbSetPatients.As<IQueryable<Patients>>().Setup(m => m.Expression).Returns(patients.AsQueryable().Expression);
        mockDbSetPatients.As<IQueryable<Patients>>().Setup(m => m.ElementType).Returns(patients.AsQueryable().ElementType);
        using var value = patients.AsQueryable().GetEnumerator();
        mockDbSetPatients.As<IQueryable<Patients>>().Setup(m => m.GetEnumerator()).Returns(value);
        _appDbContext.Setup(db => db.Patients).Returns(mockDbSetPatients.Object);

        // Mocking the Appointments DbSet to return an empty list
        // ReSharper disable once CollectionNeverUpdated.Local
        var appointments = new List<Appointments>();
        var mockDbSetAppointments = new Mock<DbSet<Appointments>>();
        mockDbSetAppointments.As<IQueryable<Appointments>>().Setup(m => m.Provider).Returns(appointments.AsQueryable().Provider);
        mockDbSetAppointments.As<IQueryable<Appointments>>().Setup(m => m.Expression).Returns(appointments.AsQueryable().Expression);
        mockDbSetAppointments.As<IQueryable<Appointments>>().Setup(m => m.ElementType).Returns(appointments.AsQueryable().ElementType);
        using var enumerator1 = appointments.AsQueryable().GetEnumerator();
        mockDbSetAppointments.As<IQueryable<Appointments>>().Setup(m => m.GetEnumerator()).Returns(enumerator1);
        _appDbContext.Setup(db => db.Appointments).Returns(mockDbSetAppointments.Object);

        // Act
        var result = _appointmentRepo.GetAppointmentsByPatient(patientId);

        // Assert
        Assert.Empty(result);
    }
    
    [Fact]
    public void GetAppointment_ReturnsAppointment_WhenAppointmentExists()
    {
        // Arrange
        const int appointmentId = 1;
        const int providerId = 1;
        const int patientId = 1;
        var appointment = new Appointments { Id = appointmentId, ProviderId = providerId,PatientId = patientId,AppointmentDate = DateTime.Now, AppointmentTime = "10:00",Status = true};

        var appointments = new List<Appointments>
        {
            appointment
        };

        var queryableAppointments = appointments.AsQueryable();

        var mockDbSetAppointments = new Mock<DbSet<Appointments>>();
        mockDbSetAppointments.As<IQueryable<Appointments>>().Setup(m => m.Provider).Returns(queryableAppointments.Provider);
        mockDbSetAppointments.As<IQueryable<Appointments>>().Setup(m => m.Expression).Returns(queryableAppointments.Expression);
        mockDbSetAppointments.As<IQueryable<Appointments>>().Setup(m => m.ElementType).Returns(queryableAppointments.ElementType);
        using var enumerator = queryableAppointments.GetEnumerator();
        mockDbSetAppointments.As<IQueryable<Appointments>>().Setup(m => m.GetEnumerator()).Returns(enumerator);

        _appDbContext.Setup(db => db.Appointments).Returns(mockDbSetAppointments.Object);
        _appDbContext.Setup(db => db.Appointments.Find(appointmentId)).Returns(appointment);

        // Mocking the Providers DbSet
        var providers = new List<Providers>
        {
            new Providers { Id = providerId }
        };
        var mockDbSetProviders = new Mock<DbSet<Providers>>();
        mockDbSetProviders.As<IQueryable<Providers>>().Setup(m => m.Provider).Returns(providers.AsQueryable().Provider);
        mockDbSetProviders.As<IQueryable<Providers>>().Setup(m => m.Expression).Returns(providers.AsQueryable().Expression);
        mockDbSetProviders.As<IQueryable<Providers>>().Setup(m => m.ElementType).Returns(providers.AsQueryable().ElementType);
        using var enumeratorProviders = providers.AsQueryable().GetEnumerator();
        mockDbSetProviders.As<IQueryable<Providers>>().Setup(m => m.GetEnumerator()).Returns(enumeratorProviders);
        _appDbContext.Setup(db => db.Providers).Returns(mockDbSetProviders.Object);

        // Mocking the Patients DbSet
        var patients = new List<Patients>
        {
            new Patients { Id = patientId }
        };
        var mockDbSetPatients = new Mock<DbSet<Patients>>();
        mockDbSetPatients.As<IQueryable<Patients>>().Setup(m => m.Provider).Returns(patients.AsQueryable().Provider);
        mockDbSetPatients.As<IQueryable<Patients>>().Setup(m => m.Expression).Returns(patients.AsQueryable().Expression);
        mockDbSetPatients.As<IQueryable<Patients>>().Setup(m => m.ElementType).Returns(patients.AsQueryable().ElementType);
        using var enumeratorPatients = patients.AsQueryable().GetEnumerator();
        mockDbSetPatients.As<IQueryable<Patients>>().Setup(m => m.GetEnumerator()).Returns(enumeratorPatients);
        _appDbContext.Setup(db => db.Patients).Returns(mockDbSetPatients.Object);

        // Act
        var result = _appointmentRepo.GetAppointment(appointmentId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(appointmentId, result.Id);
    }
    
    [Fact]
    public void GetAppointment_ReturnsNull_WhenAppointmentDoesNotExist()
    {
        // Arrange
        const int nonExistentAppointmentId = 999;
        
        // Mocking the Appointments to return null
        // ReSharper disable once CollectionNeverUpdated.Local
        var appointments = new List<Appointments>();
        var mockDbSetAppointments = new Mock<DbSet<Appointments>>();
        mockDbSetAppointments.As<IQueryable<Appointments>>().Setup(m => m.Provider).Returns(appointments.AsQueryable().Provider);
        mockDbSetAppointments.As<IQueryable<Appointments>>().Setup(m => m.Expression).Returns(appointments.AsQueryable().Expression);
        mockDbSetAppointments.As<IQueryable<Appointments>>().Setup(m => m.ElementType).Returns(appointments.AsQueryable().ElementType);
        mockDbSetAppointments.As<IQueryable<Appointments>>().Setup(m => m.GetEnumerator()).Returns(appointments.GetEnumerator());
        _appDbContext.Setup(db => db.Appointments).Returns(mockDbSetAppointments.Object);

        // Mocking the Providers DbSet
        // ReSharper disable once CollectionNeverUpdated.Local
        var providers = new List<Providers>();
        var mockDbSetProviders = new Mock<DbSet<Providers>>();
        mockDbSetProviders.As<IQueryable<Providers>>().Setup(m => m.Provider).Returns(providers.AsQueryable().Provider);
        mockDbSetProviders.As<IQueryable<Providers>>().Setup(m => m.Expression).Returns(providers.AsQueryable().Expression);
        mockDbSetProviders.As<IQueryable<Providers>>().Setup(m => m.ElementType).Returns(providers.AsQueryable().ElementType);
        mockDbSetProviders.As<IQueryable<Providers>>().Setup(m => m.GetEnumerator()).Returns(providers.GetEnumerator());
        _appDbContext.Setup(db => db.Providers).Returns(mockDbSetProviders.Object);

        // Mocking the Patients DbSet
        // ReSharper disable once CollectionNeverUpdated.Local
        var patients = new List<Patients>();
        var mockDbSetPatients = new Mock<DbSet<Patients>>();
        mockDbSetPatients.As<IQueryable<Patients>>().Setup(m => m.Provider).Returns(patients.AsQueryable().Provider);
        mockDbSetPatients.As<IQueryable<Patients>>().Setup(m => m.Expression).Returns(patients.AsQueryable().Expression);
        mockDbSetPatients.As<IQueryable<Patients>>().Setup(m => m.ElementType).Returns(patients.AsQueryable().ElementType);
        mockDbSetPatients.As<IQueryable<Patients>>().Setup(m => m.GetEnumerator()).Returns(patients.GetEnumerator());
        _appDbContext.Setup(db => db.Patients).Returns(mockDbSetPatients.Object);

        // Act
        var result = _appointmentRepo.GetAppointment(nonExistentAppointmentId);

        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public void GetAppointments_ReturnsAppointments_WhenAppointmentsExist()
    {
        // Arrange
        const int providerId = 1;
        const int patientId = 1;

        // Mocking the Providers DbSet
        var providers = new List<Providers>
        {
            new Providers { Id = providerId }
        };
        var mockDbSetProviders = new Mock<DbSet<Providers>>();
        mockDbSetProviders.As<IQueryable<Providers>>().Setup(m => m.Provider).Returns(providers.AsQueryable().Provider);
        mockDbSetProviders.As<IQueryable<Providers>>().Setup(m => m.Expression).Returns(providers.AsQueryable().Expression);
        mockDbSetProviders.As<IQueryable<Providers>>().Setup(m => m.ElementType).Returns(providers.AsQueryable().ElementType);
        using var enumeratorProviders = providers.AsQueryable().GetEnumerator();
        mockDbSetProviders.As<IQueryable<Providers>>().Setup(m => m.GetEnumerator()).Returns(enumeratorProviders);
        _appDbContext.Setup(db => db.Providers).Returns(mockDbSetProviders.Object);

        // Mocking the Patients DbSet
        var patients = new List<Patients>
        {
            new Patients { Id = patientId }
        };
        var mockDbSetPatients = new Mock<DbSet<Patients>>();
        mockDbSetPatients.As<IQueryable<Patients>>().Setup(m => m.Provider).Returns(patients.AsQueryable().Provider);
        mockDbSetPatients.As<IQueryable<Patients>>().Setup(m => m.Expression).Returns(patients.AsQueryable().Expression);
        mockDbSetPatients.As<IQueryable<Patients>>().Setup(m => m.ElementType).Returns(patients.AsQueryable().ElementType);
        using var enumeratorPatients = patients.AsQueryable().GetEnumerator();
        mockDbSetPatients.As<IQueryable<Patients>>().Setup(m => m.GetEnumerator()).Returns(enumeratorPatients);
        _appDbContext.Setup(db => db.Patients).Returns(mockDbSetPatients.Object);

        // Mocking the Appointments to return a list with some appointments
        var appointments = new List<Appointments>
        {
            new Appointments { AppointmentDate = DateTime.Now, AppointmentTime = "10:00", ProviderId = providerId, PatientId = patientId,Status = true},
            new Appointments { AppointmentDate = DateTime.Now, AppointmentTime = "11:00", ProviderId = providerId, PatientId = patientId,Status = true}
        };
        var dbSetMock = new Mock<DbSet<Appointments>>();
        dbSetMock.As<IQueryable<Appointments>>().Setup(m => m.Provider).Returns(appointments.AsQueryable().Provider);
        dbSetMock.As<IQueryable<Appointments>>().Setup(m => m.Expression).Returns(appointments.AsQueryable().Expression);
        dbSetMock.As<IQueryable<Appointments>>().Setup(m => m.ElementType).Returns(appointments.AsQueryable().ElementType);
        using var enumerator = appointments.AsQueryable().GetEnumerator();
        dbSetMock.As<IQueryable<Appointments>>().Setup(m => m.GetEnumerator()).Returns(enumerator);
        _appDbContext.Setup(db => db.Appointments).Returns(dbSetMock.Object);

        // Act
        var result = _appointmentRepo.GetAppointments();

        // Assert
        Assert.Equal(appointments.Count, result.Count);
    }
    
    [Fact]
    public void GetAppointments_ReturnsEmptyList_WhenNoAppointmentsExist()
    {
        // Arrange
        // Mocking the Appointments to return an empty list
        // ReSharper disable once CollectionNeverUpdated.Local
        var appointments = new List<Appointments>();
        var dbSetMock = new Mock<DbSet<Appointments>>();
        dbSetMock.As<IQueryable<Appointments>>().Setup(m => m.Provider).Returns(appointments.AsQueryable().Provider);
        dbSetMock.As<IQueryable<Appointments>>().Setup(m => m.Expression).Returns(appointments.AsQueryable().Expression);
        dbSetMock.As<IQueryable<Appointments>>().Setup(m => m.ElementType).Returns(appointments.AsQueryable().ElementType);
        using var enumerator = new List<Appointments>().AsQueryable().GetEnumerator();
        dbSetMock.As<IQueryable<Appointments>>().Setup(m => m.GetEnumerator()).Returns(enumerator);
        _appDbContext.Setup(db => db.Appointments).Returns(dbSetMock.Object);

        // Mocking the Providers DbSet
        // ReSharper disable once CollectionNeverUpdated.Local
        var providers = new List<Providers>();
        var mockDbSetProviders = new Mock<DbSet<Providers>>();
        mockDbSetProviders.As<IQueryable<Providers>>().Setup(m => m.Provider).Returns(providers.AsQueryable().Provider);
        mockDbSetProviders.As<IQueryable<Providers>>().Setup(m => m.Expression).Returns(providers.AsQueryable().Expression);
        mockDbSetProviders.As<IQueryable<Providers>>().Setup(m => m.ElementType).Returns(providers.AsQueryable().ElementType);
        using var enumeratorProviders = providers.AsQueryable().GetEnumerator();
        mockDbSetProviders.As<IQueryable<Providers>>().Setup(m => m.GetEnumerator()).Returns(enumeratorProviders);
        _appDbContext.Setup(db => db.Providers).Returns(mockDbSetProviders.Object);

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
        var result = _appointmentRepo.GetAppointments();

        // Assert
        Assert.Empty(result);
    }
    
    [Fact]
    public void UpdateAppointment_ReturnsFailure_WhenAppointmentDoesNotExist()
    {
        // Arrange
        const int nonExistentAppointmentId = 999;
        
        // Mocking the Appointments to return null
        _appDbContext.Setup(db => db.Appointments.Find(nonExistentAppointmentId)).Returns((Appointments)null!);

        // Act
        var result = _appointmentRepo.UpdateAppointment(new AppointmentDto { Id = nonExistentAppointmentId });

        // Assert
        Assert.False(result.Flag);
        Assert.Equal("Appointment not found", result.Message);
    }
    
    [Fact]
    public void UpdateAppointment_ReturnsSuccess_WhenAppointmentIsUpdatedSuccessfully()
    {
        // Arrange
        const int appointmentId = 1;
        var appointment = new Appointments { Id = appointmentId };
        _appDbContext.Setup(db => db.Appointments.Find(appointmentId)).Returns(appointment);

        // Act
        var result = _appointmentRepo.UpdateAppointment(new AppointmentDto { Id = appointmentId });

        // Assert
        Assert.True(result.Flag);
        Assert.Equal("Appointment updated successfully", result.Message);
    }
    
    [Fact]
    public void DeleteAppointment_ReturnsFailure_WhenAppointmentDoesNotExist()
    {
        // Arrange
        const int nonExistentAppointmentId = 999;
        
        // Mocking the Appointments to return null
        _appDbContext.Setup(db => db.Appointments.Find(nonExistentAppointmentId)).Returns((Appointments)null!);

        // Act
        var result = _appointmentRepo.DeleteAppointment(nonExistentAppointmentId);

        // Assert
        Assert.False(result.Flag);
        Assert.Equal("Appointment not found", result.Message);
    }
    
    [Fact]
    public void DeleteAppointment_ReturnsSuccess_WhenAppointmentIsDeletedSuccessfully()
    {
        // Arrange
        const int appointmentId = 1;
        var appointment = new Appointments { Id = appointmentId };
        _appDbContext.Setup(db => db.Appointments.Find(appointmentId)).Returns(appointment);

        // Act
        var result = _appointmentRepo.DeleteAppointment(appointmentId);

        // Assert
        Assert.True(result.Flag);
        Assert.Equal("Appointment deleted successfully", result.Message);
    }
}