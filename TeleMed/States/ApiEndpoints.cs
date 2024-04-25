using TeleMed.Common.Extensions;
using TeleMed.Controllers;

namespace TeleMed.States;

public static class ApiEndpoints
{
    public static readonly string AccountsApi = nameof(AccountsController).FormatControllerName();
    public static readonly string AppointmentsApi = nameof(AppointmentsController).FormatControllerName();
    public static readonly string PatientsApi = nameof(PatientsController).FormatControllerName();
    public static readonly string ProvidersApi = nameof(ProvidersController).FormatControllerName();
}