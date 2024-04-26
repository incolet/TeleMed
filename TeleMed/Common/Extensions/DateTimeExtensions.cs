namespace TeleMed.Common.Extensions;

public static class DateTimeExtensions
{
    public static string ToTimeString(this DateTime dateTime)
    {
        var amPmDesignator = dateTime.Hour < 12 ? "AM" : "PM";
        var hour = dateTime.Hour % 12;
        if (hour == 0)
            hour = 12;

        var timeString = $"{hour}:{dateTime.Minute:D2} {amPmDesignator}";
        return timeString;
    }
}