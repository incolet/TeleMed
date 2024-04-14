using TeleMed.DTOs;
using TeleMed.Responses;
using System;
using TeleMed.Data;
using static TeleMed.Responses.CustomResponses;
using TeleMed.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace TeleMed.Repos
{
    public class Account : IAccount
    {
        private readonly AppDbContext appDbContext;
        private readonly IConfiguration config;

        public Account(AppDbContext appDbContext, IConfiguration config)
        {
            this.appDbContext = appDbContext;
            this.config = config;
        }
        public LoginResponse LoginAsync(LoginDTO model)
        {

            var findUser = GetUser(model.Email);
            if (findUser == null) 
                return new LoginResponse(false, "User doesn't exist");

            if (!BCrypt.Net.BCrypt.Verify(model.Password, findUser?.Password))
                return new LoginResponse(false, "Email/Password not valid");
            
            string jwtToken = GenerateToken(findUser!);

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

        public async Task<RegistrationResponse> RegisterAsync(RegisterDTO model)
        {
            var findUser =  GetUser(model.Email);
            if (findUser != null) return new RegistrationResponse(false, "User already exist");

            appDbContext.Users.Add(new ApplicationUser()
            {
                Name = model.Name,
                Email = model.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password)
            });

            await appDbContext.SaveChangesAsync();
            return new RegistrationResponse(true, "Success");
        }

        private ApplicationUser GetUser(string email)
        {
            var user = appDbContext.Users.FirstOrDefaultAsync(e => e.Email == email).Result;

            return user ?? null!;
        }
    }
}
