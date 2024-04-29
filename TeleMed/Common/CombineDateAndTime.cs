namespace TeleMed.Common;

public static class CombineDateAndTime
{
    public static DateTime Combine(DateTime appointmentDate, string appointmentTime)
    {
        // Parse the time string into a TimeSpan object
        var timeSpan = DateTime.Parse(appointmentTime).TimeOfDay;

        // Combine the date and time into a single DateTime object
        var combinedDateTime = appointmentDate.Date + timeSpan;

        return combinedDateTime;
    }
}