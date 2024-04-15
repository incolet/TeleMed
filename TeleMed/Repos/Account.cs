using TeleMed.Data;
using static TeleMed.Responses.CustomResponses;
using TeleMed.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using TeleMed.DTOs.Auth;
using TeleMed.Repos.Abstracts;
using TeleMed.States;

namespace TeleMed.Repos
{
    public class Account(AppDbContext appDbContext, IConfiguration config) : IAccount
    {
        public LoginResponse LoginAsync(LoginDTO model)
        {

            var findUser = GetUser(model.Email);
            if (findUser.Id < 1) 
                return new LoginResponse(false, "User doesn't exist");

            if (!BCrypt.Net.BCrypt.Verify(model.Password, findUser.Password))
                return new LoginResponse(false, "Email/Password not valid");
            
            string jwtToken = GenerateToken(findUser);

            return new LoginResponse(true, "Login Success" , jwtToken);
        }

        private string GenerateToken(ApplicationUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var userClaims = new[]
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
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
            var findUser =  GetUser(model.Email);
            if (findUser.Id > 1) 
                return (new RegistrationResponse(false, "User already exist"),findUser.Id);

            var newUser = new ApplicationUser()
            {
                Name = model.Name,
                Email = model.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password)

            };
            
            appDbContext.Users.Add(newUser);
            appDbContext.SaveChangesAsync();
            
            var recordId = newUser.Id;
            return (new RegistrationResponse(true, "Success"),recordId);
        }

        public ApplicationUser GetUser(string email)
        {
           var user = appDbContext.Users.FirstOrDefaultAsync(e => e.Email == email).Result;

           return user ?? null!;
        }
        public ApplicationUser GetUser(int id)
        {
           var user = appDbContext.Users.FirstOrDefaultAsync(e => e.Id == id).Result;

           return user ?? null!;
        }

        public LoginResponse RefreshToken(UserSession userSession)
        {
            CustomUserClaims getUserClaims = DecryptJwtToken.DecryptToken(userSession.JwtToken);
            if (string.IsNullOrEmpty(getUserClaims.Name) || string.IsNullOrEmpty(getUserClaims.Email))
                return new LoginResponse(false, "Invalid Token");
            
            string newToken = GenerateToken(new ApplicationUser()
            {
                Name = getUserClaims.Name,
                Email = getUserClaims.Email,
                Role = getUserClaims.Role
            });
            
            return new LoginResponse(true, "Token Refreshed", newToken);
            
        }
    }
}
