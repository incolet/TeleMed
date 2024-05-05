using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;
using Serilog.Events;
using TeleMed.Components;
using TeleMed.Data;
using TeleMed.Data.Abstracts;
using TeleMed.Repos;
using TeleMed.Repos.Abstracts;
using TeleMed.Services;
using TeleMed.Services.Abstracts;
using TeleMed.States;


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    // ReSharper disable once StringLiteralTypo
    .WriteTo.File("Logs/log.txt",rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Starting the service...");
    
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents()
        .AddInteractiveWebAssemblyComponents();

    builder.Services.AddControllers();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Telemedicine API", Version = "v1" });
    });

    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    });

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

    //Inject Repositories

    builder.Services.AddScoped<IAppDbContext, AppDbContext>();

    builder.Services.AddScoped<IPatient,Patient>();
    builder.Services.AddScoped<IProvider,Provider>();
    builder.Services.AddScoped<IAccount,PatientAccount>();
    builder.Services.AddScoped<IAccount,ProviderAccount>();
    builder.Services.AddScoped<IAppointment,Appointment>();
    builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration["API_URL"]!) });

    //Inject Services
    builder.Services.AddScoped<IPatientsService, PatientsService>();
    builder.Services.AddScoped<IProvidersService, ProvidersService>();
    builder.Services.AddScoped<IAccountsService, AccountsService>();
    builder.Services.AddScoped<IAppointmentsService, AppointmentsService>();
    builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
    builder.Services.AddSingleton<CircuitHandlerService>();
    builder.Services.AddBlazorBootstrap();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseWebAssemblyDebugging();
    }
    else
    {
        app.UseExceptionHandler("/Error", createScopeForErrors: true);
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();

    app.UseStaticFiles();
    app.UseAntiforgery();

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Telemedicine API V1");
    });

    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode()
        .AddInteractiveWebAssemblyRenderMode();

    app.MapControllers();
    app.UseAuthentication();
    app.UseAuthorization();
    
    app.Run();

    Log.Information("Service started successfully");
}
catch (Exception e)
{
    Log.Fatal(e, "There was a problem starting the service");

}
finally
{
    Log.CloseAndFlush();
}



