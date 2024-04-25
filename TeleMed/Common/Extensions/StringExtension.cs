namespace TeleMed.Common.Extensions;

public static class StringExtension
{
    
        public static string FormatControllerName(this string input)
        { 
                ArgumentNullException.ThrowIfNull(input);
                
                return $"api/{input.Replace("Controller", string.Empty)}";
        }
}