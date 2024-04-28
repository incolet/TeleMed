using TeleMed.DTOs.GoogleMeet;

namespace TeleMed.Repos.Abstracts;

public interface IGoogleMeet
{
    Task<string> GetGoogleMeetLink(GoogleMeetRequestDto googleMeetRequestDto);
    
    void CancelGoogleMeet(string googleMeetLink);
}