using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using TeleMed.DTOs.GoogleMeet;
using TeleMed.Repos.Abstracts;

namespace TeleMed.Repos;

public class GoogleMeet : IGoogleMeet
{
    private const string CredentialsFilePath = "../../../credentials.json";
    
    public async Task<string> GetGoogleMeetLink(GoogleMeetRequestDto model)
    {
        // Event details 
        const string meetingTitle = "Session with Doctor";
        var startTime = model.MeetingDateTime;
        var endTime = startTime.AddMinutes(30);

        // Create Google Calendar API service
        var service = await GetCalendarService(CredentialsFilePath);

        // Create new event object
        var newEvent = new Event()
        {
            Summary = meetingTitle,
            Start = new EventDateTime() { DateTimeDateTimeOffset = startTime },
            End = new EventDateTime() { DateTimeDateTimeOffset = endTime },
            Attendees = new List<EventAttendee>()
            {
                new () { Email = model.PatientEmail!}
            },
            ConferenceData = new ConferenceData()
            {
                CreateRequest = new CreateConferenceRequest()
                {
                    RequestId = Guid.NewGuid().ToString(),
                    ConferenceSolutionKey = new ConferenceSolutionKey() { Type = "hangoutsMeet" }
                }
            }
        };

        // Insert event and get the response
        var createdEvent = service.Events.Insert(newEvent, "primary");
        createdEvent.ConferenceDataVersion = 1;
        var eventResponse = await createdEvent.ExecuteAsync();
        
      
        // Extract the Google Meet link from the response
        var meetLink = eventResponse.HangoutLink;
        
        return meetLink;

    }
    
    public void CancelGoogleMeet(string googleMeetLink)
    {
        // Extract the event ID from the Google Meet link
        var eventId = googleMeetLink.Split("/").Last();
        
        // Create Google Calendar API service
        var service = GetCalendarService(CredentialsFilePath).Result;

        // Delete the event
        service.Events.Delete("primary", eventId).Execute();
    }
   private static async Task<CalendarService> GetCalendarService(string credentialsFilePath)
    {
        // Create Google credential object from JSON file
        // credentials.json points to the file containing the client ID and client secret.
        var clientSecrets = await GoogleClientSecrets.FromFileAsync(credentialsFilePath);

        var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
            clientSecrets.Secrets,
            new[] { CalendarService.Scope.Calendar },
            "tunjiincolet@gmail.com",
            CancellationToken.None);

        // Create Google Calendar API service object
        var service = new CalendarService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential
        });

        return service;
    }
}