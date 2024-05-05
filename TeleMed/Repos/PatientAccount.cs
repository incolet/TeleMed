
using TeleMed.Data.Abstracts;
using TeleMed.DTOs.Auth;
using TeleMed.Responses;
using TeleMed.States;

namespace TeleMed.Repos;

public class PatientAccount(IAppDbContext appDbContext, IConfiguration configuration) : AAccount(appDbContext, configuration)
{
    public override CustomResponses.LoginResponse LoginAsync(LoginDto model)
    {
        var findUser = GetUser(model);
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (findUser is null)
            return new CustomResponses.LoginResponse(false, "User doesn't exist");
        if (!IsUserInRole(findUser, (int)UserRoles.Patient))
        {
            return (new CustomResponses.LoginResponse(false, "User doesn't exist"));
        }

        if (!BCrypt.Net.BCrypt.Verify(model.Password, findUser.Password))
            return new CustomResponses.LoginResponse(false, "Email/Password not valid");
            
        var jwtToken = GenerateToken(findUser);

        return new CustomResponses.LoginResponse(true, "Login Success" , jwtToken);
    }
}