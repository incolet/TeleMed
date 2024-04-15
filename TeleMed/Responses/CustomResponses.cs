namespace TeleMed.Responses
{
    public class CustomResponses
    {
        public record RegistrationResponse(bool Flag = false, string Message = null!);
        public record LoginResponse(bool Flag = false, string Message = null!, string JWTToken = null!);
        public record AppointmentResponse(bool Flag = false, string Message = null!);
        public record PatientResponse(bool Flag = false, string Message = null!);
        public record ProviderResponse(bool Flag = false, string Message = null!);
    }
}
