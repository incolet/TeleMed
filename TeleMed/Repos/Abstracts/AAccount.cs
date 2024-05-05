using TeleMed.Data;
using static TeleMed.Responses.CustomResponses;
using TeleMed.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using TeleMed.Data.Abstracts;
using TeleMed.DTOs.Auth;
using TeleMed.Repos.Abstracts;
using TeleMed.Responses;
using TeleMed.States;

namespace TeleMed.Repos
{
    public abstract class AAccount(IAppDbContext appDbContext, IConfiguration config) : IAccount
    {
        public abstract LoginResponse LoginAsync(LoginDto model);

        protected string GenerateToken(ApplicationUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var userClaims = new[]
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };
            var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: userClaims,
                expires: DateTime.Now.AddDays(2),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public (RegistrationResponse,int) RegisterAsync(RegisterDto model)
        {
            var loginDto = new LoginDto()
            {
                Email = model.Email,
            };
            var findUser =  GetUser(loginDto);
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (findUser is not null) 
                return (new RegistrationResponse(false, "User already exist"),findUser.Id);

            var newUser = new ApplicationUser()
            {
                Name = model.Name,
                Email = model.Email,
                Role = model.Role,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password)

            };
            
            appDbContext.Users.Add(newUser);
            appDbContext.SaveChanges();

            var recordId = newUser.Id;
            return (new RegistrationResponse(true, "Success"),recordId);
        }

        public ApplicationUser GetUser(LoginDto model)
        {
           var user = appDbContext.Users
               .FirstOrDefaultAsync(e => e.Email == model.Email).Result;

           return user ?? null!;
        }
        
        public bool IsUserInRole(ApplicationUser user, int role)
        {
            return user.Role == role;
        }
        
        public ApplicationUser GetUser(int id)
        {
           var user = appDbContext.Users.FirstOrDefaultAsync(e => e.Id == id).Result;

           return user ?? null!;
        }

        public LoginResponse RefreshToken(UserSession userSession)
        {
            var getUserClaims = DecryptJwtToken.DecryptToken(userSession.JwtToken);
            if (string.IsNullOrEmpty(getUserClaims.Name) || string.IsNullOrEmpty(getUserClaims.Email) )
                return new LoginResponse(false, "Invalid Token");
            
            var newToken = GenerateToken(new ApplicationUser()
            {
                Name = getUserClaims.Name,
                Email = getUserClaims.Email,
                Role = getUserClaims.Role,
                Id = int.TryParse(getUserClaims.UniqueId, out var id) ? id : 0
            });
            
            return new LoginResponse(true, "Token Refreshed", newToken);
            
        }
    }
}
