    using TeleMed.Data;
    using TeleMed.Data.Abstracts;
    using TeleMed.DTOs.Appointment;
    using TeleMed.Repos.Abstracts;
    using TeleMed.Responses;
    using TeleMed.States;

    namespace TeleMed.Repos;

    public class Appointment (IAppDbContext appDbContext)
        : IAppointment
    {
        
        public CustomResponses.AppointmentResponse CreateAppointment(AppointmentDto appointmentDto)
        {

        // Check if the provider exists
        var provider = appDbContext.Providers.Find(appointmentDto.ProviderId);
        if (provider == null)
        {
            return new CustomResponses.AppointmentResponse(false, "Provider not found");
        }

        // Check if the appointment time is in the future
        if (appointmentDto.AppointmentDate < DateTime.Now)
        {
            return new CustomResponses.AppointmentResponse(false, "Appointment time must be in the future");
        }

        var appointments = appDbContext.Appointments
            .Where(a => a.ProviderId == appointmentDto.ProviderId 
                        && a.AppointmentDate == appointmentDto.AppointmentDate)
            .ToList();

        var existingAppointment = appointments
            .FirstOrDefault(a => a.AppointmentTime == appointmentDto.AppointmentTime);  
        if (existingAppointment != null)
        {
            return new CustomResponses.AppointmentResponse(false, "Provider already has an appointment at this time");
        }

        var appointment = new Models.Appointments
        {
            PatientId = appointmentDto.PatientId,
            ProviderId = appointmentDto.ProviderId,
            AppointmentDate = appointmentDto.AppointmentDate,
            AppointmentTime = appointmentDto.AppointmentTime,
            AppointmentStatus = (int)AppointmentStatus.Scheduled
        };

        appDbContext.Appointments.Add(appointment);
        appDbContext.SaveChanges();

        return new CustomResponses.AppointmentResponse(true, "Appointment created successfully");
    }

        public CustomResponses.AppointmentResponse UpdateAppointment(AppointmentDto appointmentDto)
        {
            var appointment = appDbContext.Appointments.Find(appointmentDto.Id);
            if (appointment is null)
                return new CustomResponses.AppointmentResponse(false, "Appointment not found");
            
            appointment.AppointmentDate = appointmentDto.AppointmentDate;
            appointment.AppointmentTime = appointmentDto.AppointmentTime;
            
            appDbContext.Appointments.Update(appointment);
            appDbContext.SaveChanges();
            return new CustomResponses.AppointmentResponse(true, "Appointment updated successfully");
        }

        public CustomResponses.AppointmentResponse DeleteAppointment(int appointmentId)
        {
            var appointment = appDbContext.Appointments.Find(appointmentId);
            if (appointment is null)
                return new CustomResponses.AppointmentResponse(false, "Appointment not found");
            
            appointment.Status = false;
            
            appDbContext.Appointments.Update(appointment);
            appDbContext.SaveChanges();
            
            return new CustomResponses.AppointmentResponse(true, "Appointment deleted successfully");
        }

        public AppointmentDto GetAppointment(int appointmentId)
        {
            var query = from a in appDbContext.Appointments
                join p in appDbContext.Patients on a.PatientId equals p.Id
                join pr in appDbContext.Providers on a.ProviderId equals pr.Id
                where a.Id == appointmentId
                select new AppointmentDto()
                {
                    Id = a.Id,
                    AppointmentDate = a.AppointmentDate,
                    AppointmentTime = a.AppointmentTime,
                    PatientId = a.PatientId,
                    PatientName = $"{p.FirstName}, {p.LastName}",
                    ProviderId = a.ProviderId,
                    ProviderName = $"{pr.FirstName}, {pr.LastName}",
                    AppointmentStatus = Enum.GetName(typeof(AppointmentStatus), a.AppointmentStatus)!
                };
            
            return query.FirstOrDefault() ?? null!;
        }

        public List<AppointmentDto> GetAppointments()
        {
            var query = from a in appDbContext.Appointments
                join p in appDbContext.Patients on a.PatientId equals p.Id
                join pr in appDbContext.Providers on a.ProviderId equals pr.Id
                select new AppointmentDto()
                {
                    Id = a.Id,
                    AppointmentDate = a.AppointmentDate,
                    AppointmentTime = a.AppointmentTime,
                    PatientId = a.PatientId,
                    PatientName = $"{p.FirstName}, {p.LastName}",
                    ProviderId = a.ProviderId,
                    ProviderName = $"{pr.FirstName}, {pr.LastName}",
                    AppointmentStatus = Enum.GetName(typeof(AppointmentStatus), a.AppointmentStatus)!
                };
                
            return query.ToList();
        }

        public List<AppointmentDto> GetAppointmentsByPatient(int patientId)
        {
            var appointmentStatus = typeof(AppointmentStatus);
            var query = from a in appDbContext.Appointments
                join p in appDbContext.Patients on a.PatientId equals p.Id
                join pr in appDbContext.Providers on a.ProviderId equals pr.Id
                where a.PatientId == patientId
                select new AppointmentDto()
                {
                    Id = a.Id,
                    AppointmentDate = a.AppointmentDate,
                    AppointmentTime = a.AppointmentTime,
                    PatientId = a.PatientId,
                    PatientName = $"{p.FirstName}, {p.LastName}",
                    ProviderId = a.ProviderId,
                    ProviderName = $"{pr.FirstName}, {pr.LastName}",
                    AppointmentStatus = Enum.GetName(appointmentStatus, a.AppointmentStatus)!
                };
            
            return query.ToList();
        }

        public List<AppointmentDto> GetAppointmentsByProvider(int providerId)
        {
            var query = from a in appDbContext.Appointments
                join p in appDbContext.Patients on a.PatientId equals p.Id
                join pr in appDbContext.Providers on a.ProviderId equals pr.Id
                where a.ProviderId == providerId
                select new AppointmentDto()
                {
                    Id = a.Id,
                    AppointmentDate = a.AppointmentDate,
                    AppointmentTime = a.AppointmentTime,
                    PatientId = a.PatientId,
                    PatientName = $"{p.FirstName}, {p.LastName}",
                    ProviderId = a.ProviderId,
                    ProviderName = $"{pr.FirstName}, {pr.LastName}",
                    AppointmentStatus = Enum.GetName(typeof(AppointmentStatus), a.AppointmentStatus)!
                };
            
            return query.ToList();
        }

        public CustomResponses.AppointmentResponse CancelAppointment(int appointmentId)
        {
            var appointment = appDbContext.Appointments.Find(appointmentId);
            if (appointment is null)
                return new CustomResponses.AppointmentResponse(false, "Appointment not found");
            
            appointment.AppointmentStatus = (int)AppointmentStatus.Cancelled;
            
            appDbContext.Appointments.Update(appointment);
            appDbContext.SaveChanges();
            
            return new CustomResponses.AppointmentResponse(true, "Appointment cancelled successfully");
        }

        public CustomResponses.AppointmentResponse RescheduleAppointment(int appointmentId, DateTime newDate, string newTime)
        {
            var appointment = appDbContext.Appointments.Find(appointmentId);
            if (appointment is null)
                return new CustomResponses.AppointmentResponse(false, "Appointment not found");
            
            appointment.AppointmentDate = newDate;
            appointment.AppointmentTime = newTime;
            
            appDbContext.Appointments.Update(appointment);
            appDbContext.SaveChanges();
            
            return new CustomResponses.AppointmentResponse(true, "Appointment rescheduled successfully");
        }

        public List<DateTime> GetAvailableTimeSlots(int providerId, DateTime date)
        {
            var provider = appDbContext.Providers.Find(providerId);
            if (provider is null)
                return [];

            var appointments = appDbContext.Appointments
                .Where(a => a.ProviderId == providerId && a.AppointmentDate == date)
                .Select(a => a.AppointmentTime)
                .ToList();

            var availableTimeSlots = Enumerable.Range(AppointmentConstants.StartTime, AppointmentConstants.EndTime - AppointmentConstants.StartTime)
                .SelectMany(hour => Enumerable.Range(0, 60 / AppointmentConstants.AppointmentDuration)
                    .Select(min => date.Date.AddHours(hour).AddMinutes(min * AppointmentConstants.AppointmentDuration)))
                .Where(timeSlot => !appointments.Contains(timeSlot.ToString("HH:mm")))
                .ToList();

            return availableTimeSlots;
        }
    }