using TeleMed.Common.Extensions;
using TeleMed.DTOs.Appointment;
using TeleMed.Responses;
using TeleMed.Services.Abstracts;

using API = TeleMed.States.ApiEndpoints;
namespace TeleMed.Services;

class AppointmentsService(
    ILogger<AppointmentsService> logger,
    HttpClient httpClient,
    IServiceProvider serviceProvider)
    : BaseService<AppointmentsService>(logger, httpClient, serviceProvider), IAppointmentsService
{
    private readonly string _baseUrl = API.AppointmentsApi;
    
    public async Task<CustomResponses.AppointmentResponse> CreateAppointment(AppointmentDto appointmentDto)
    {
        var response = await HttpClient.ExtSendRequestAsync(new HttpRequestMessage(HttpMethod.Post,$"{_baseUrl}"),appointmentDto);
        var result = await response.Content.ReadFromJsonAsync<CustomResponses.AppointmentResponse>();
        return result!;
    }

    public async Task<List<AppointmentDto>> GetAppointmentsByPatientId(int patientId)
    {
        var response =  HttpClient.ExtSendRequestAsyncNoBody(new HttpRequestMessage(HttpMethod.Get,$"{_baseUrl}/patient/{patientId}")).Result;
        var result = await response.Content.ReadFromJsonAsync<List<AppointmentDto>>();
        return result!;
    }
    
    public async Task<List<DateTime>> GetAvailableAppointmentTimes(int providerId, string appointmentDate)
    {
        var response =  HttpClient
            .ExtSendRequestAsyncNoBody(new HttpRequestMessage(HttpMethod.Get,$"{_baseUrl}/time-slots/{providerId}/{appointmentDate}")).Result;
        var result = await response.Content.ReadFromJsonAsync<List<DateTime>>();
        return result!;
    }
    
    public async Task<List<AppointmentDto>> GetAppointmentsByProviderId(int providerId)
    {
        var response =  HttpClient.ExtSendRequestAsyncNoBody(new HttpRequestMessage(HttpMethod.Get,$"{_baseUrl}/provider/{providerId}")).Result;
        var result = await response.Content.ReadFromJsonAsync<List<AppointmentDto>>();
        return result!;
    }
}