using TeleMed.Common.Extensions;
using TeleMed.DTOs.Patient;
using TeleMed.Responses;
using TeleMed.Services.Abstracts;
using API = TeleMed.States.ApiEndpoints;
namespace TeleMed.Services;

public class PatientsService(ILogger<PatientsService> logger, HttpClient httpClient, IServiceProvider serviceProvider) 
    : BaseService<PatientsService>(logger,httpClient,serviceProvider) , IPatientsService
{
    private readonly string _baseUrl = API.PatientsApi;
    public async Task<CustomResponses.PatientResponse> CreatePatient(PatientRegisterDto model)
    {
        var response = await HttpClient.ExtSendRequestAsync(new HttpRequestMessage(HttpMethod.Post,$"{_baseUrl}"),model, false);
        var result = await response.Content.ReadFromJsonAsync<CustomResponses.PatientResponse>();
        return result!;
    }
}