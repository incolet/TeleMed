    using Microsoft.EntityFrameworkCore;
    using TeleMed.Common;
    using TeleMed.Common.Extensions;
    using TeleMed.Components;
    using TeleMed.Data;
    using TeleMed.Data.Abstracts;
    using TeleMed.DTOs.Appointment;
    using TeleMed.DTOs.GoogleMeet;
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
                            && a.AppointmentDate == appointmentDto.AppointmentDate.ToUniversalTime())
                .ToList();

            var existingAppointment = appointments
                .FirstOrDefault(a => a.AppointmentTime == appointmentDto.AppointmentTime);  
            if (existingAppointment != null)
            {
                return new CustomResponses.AppointmentResponse(false, "Provider already has an appointment at this time");
            }

            var appointment = new Models.Appointments
            {
                PatientId = GetPatientId(appointmentDto.PatientId),
                ProviderId = appointmentDto.ProviderId,
                AppointmentDate = appointmentDto.AppointmentDate.ToUniversalTime(),
                AppointmentTime = appointmentDto.AppointmentTime,
                AppointmentStatus = (int)AppointmentStatus.Scheduled,
                Status = true
            };

            appDbContext.Appointments.Add(appointment);

            try
            {
                var result = appDbContext.SaveChanges();
                
                if (result > 0)
                {
                    var meetingDto = new GoogleMeetRequestDto()
                    {
                        MeetingDateTime = CombineDateAndTime.Combine(appointmentDto.AppointmentDate, appointmentDto.AppointmentTime),
                        PatientEmail = appDbContext.Patients.Find(appointment.PatientId)?.Email
                    };
                    
                    UpdateAppointmentMeetingLink(appointment.Id, meetingDto);
                }
                
            }
            catch (Exception ex)
            {
                appDbContext.ChangeTracker.Clear();
                //Log the exception
                return new CustomResponses.AppointmentResponse(false, "An error occurred while creating the appointment");
            }
            
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
                where a.Id == appointmentId && a.Status == true
                select new AppointmentDto()
                {
                    Id = a.Id,
                    AppointmentDate = a.AppointmentDate,
                    AppointmentTime = a.AppointmentTime,
                    PatientId = a.PatientId,
                    PatientName = $"{p.FirstName}, {p.LastName}",
                    ProviderId = a.ProviderId,
                    ProviderName = $"{pr.FirstName}, {pr.LastName}",
                    AppointmentStatus = Enum.GetName(typeof(AppointmentStatus), a.AppointmentStatus)!,
                    MeetingLink = a.MeetingLink
                };
            
            return query.FirstOrDefault() ?? null!;
        }

        public List<AppointmentDto> GetAppointments()
        {
            var query = from a in appDbContext.Appointments
                join p in appDbContext.Patients on a.PatientId equals p.Id
                join pr in appDbContext.Providers on a.ProviderId equals pr.Id
                where a.Status == true
                select new AppointmentDto()
                {
                    Id = a.Id,
                    AppointmentDate = a.AppointmentDate,
                    AppointmentTime = a.AppointmentTime,
                    PatientId = a.PatientId,
                    PatientName = $"{p.FirstName}, {p.LastName}",
                    ProviderId = a.ProviderId,
                    ProviderName = $"{pr.FirstName}, {pr.LastName}",
                    AppointmentStatus = Enum.GetName(typeof(AppointmentStatus), a.AppointmentStatus)!,
                    MeetingLink = a.MeetingLink
                };
                
            return query.ToList();
        }

        public List<AppointmentDto> GetAppointmentsByPatient(int userId)
        {
            var patientId = GetPatientId(userId);
            
            var appointmentStatus = typeof(AppointmentStatus);
            var query = from a in appDbContext.Appointments
                join p in appDbContext.Patients on a.PatientId equals p.Id
                join pr in appDbContext.Providers on a.ProviderId equals pr.Id
                where a.PatientId == patientId && a.Status == true
                select new AppointmentDto()
                {
                    Id = a.Id,
                    AppointmentDate = a.AppointmentDate,
                    AppointmentTime = a.AppointmentTime,
                    PatientId = a.PatientId,
                    PatientName = $"{p.FirstName}, {p.LastName}",
                    ProviderId = a.ProviderId,
                    ProviderName = $"{pr.FirstName}, {pr.LastName}",
                    AppointmentStatus = Enum.GetName(appointmentStatus, a.AppointmentStatus)!,
                    MeetingLink = a.MeetingLink
                };
            
            return query.ToList();
        }

        public List<AppointmentDto> GetAppointmentsByProvider(int providerId)
        {
            var query = from a in appDbContext.Appointments
                join p in appDbContext.Patients on a.PatientId equals p.Id
                join pr in appDbContext.Providers on a.ProviderId equals pr.Id
                where a.ProviderId == providerId && a.Status == true
                select new AppointmentDto()
                {
                    Id = a.Id,
                    AppointmentDate = a.AppointmentDate,
                    AppointmentTime = a.AppointmentTime,
                    PatientId = a.PatientId,
                    PatientName = $"{p.FirstName}, {p.LastName}",
                    ProviderId = a.ProviderId,
                    ProviderName = $"{pr.FirstName}, {pr.LastName}",
                    AppointmentStatus = Enum.GetName(typeof(AppointmentStatus), a.AppointmentStatus)!,
                    MeetingLink = a.MeetingLink
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
            
            var meetingLink = appointment.MeetingLink;
            if (!string.IsNullOrWhiteSpace(meetingLink))
            {
                new GoogleMeet().CancelGoogleMeet(meetingLink);
            }
            
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
                .Where(a => a.ProviderId == providerId && a.AppointmentDate == date.ToUniversalTime().Date)
                .Select(a => a.AppointmentTime)
                .ToList();

            var availableTimeSlots = Enumerable.Range(AppointmentConstants.StartTime, AppointmentConstants.EndTime - AppointmentConstants.StartTime)
                .SelectMany(hour => Enumerable.Range(0, 60 / AppointmentConstants.AppointmentDuration)
                    .Select(min => date.Date.AddHours(hour).AddMinutes(min * AppointmentConstants.AppointmentDuration)))
                .Where(timeSlot => !appointments.Contains(timeSlot.ToTimeString()))
                .ToList();

            return availableTimeSlots;
        }
        
        private int GetPatientId(int userId)
        {
            return appDbContext.Patients.FirstOrDefault(p => p.UserId == userId)?.Id ?? 0;
        }
        
        private async void UpdateAppointmentMeetingLink(int appointmentId, GoogleMeetRequestDto meetingDto)
        {
            //Need to Catch exceptions
            var googleMeetLink = new GoogleMeet().GetGoogleMeetLink(meetingDto).Result;
            if (string.IsNullOrWhiteSpace(googleMeetLink)) return;
            var appointment = appDbContext.Appointments.Find(appointmentId);
            appointment!.MeetingLink = googleMeetLink;
                
            const string propertyName = nameof(appointment.MeetingLink);
            appDbContext.Entry(appointment).Property(propertyName).IsModified = true;
            appDbContext.SaveChanges();
        }
    }