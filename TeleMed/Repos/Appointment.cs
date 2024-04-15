    using TeleMed.Data;
    using TeleMed.DTOs;
    using TeleMed.DTOs.Appointment;
    using TeleMed.Repos.Abstracts;
    using TeleMed.Responses;
    using TeleMed.States;

    namespace TeleMed.Repos;

    public class Appointment (AppDbContext appDbContext)
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

        // Check if the provider already has an appointment at the same time
        var existingAppointment = appDbContext.Appointments
            .FirstOrDefault(a => a.ProviderId == appointmentDto.ProviderId 
                                 && a.AppointmentDate == appointmentDto.AppointmentDate
                                 && a.AppointmentTime == appointmentDto.AppointmentTime);
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
            
            return query.FirstOrDefault() ?? new AppointmentDto();
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
                    AppointmentStatus = Enum.GetName(typeof(AppointmentStatus), a.AppointmentStatus)!
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
    }