using Azure.Security.KeyVault.Secrets;
using Microsoft.EntityFrameworkCore;
using PlanePal.DbContext;
using PlanePal.Repositories.Implementation;
using PlanePal.Repositories.Interfaces;
using PlanePal.Services.Implementation;
using PlanePal.Services.Interfaces;
using PlanePal.Services.Shared;

namespace PlanePal.Config
{
    public static class ServiceConfig
    {
        public static void AddServices(WebApplicationBuilder builder)
        {
            KeyVaultSecret secret = AzureKeyVaultClientProvider.GetClient().GetSecret("ConnectionStrings");
            builder.Services.AddDbContext<PlanePalDbContext>(options =>
                    options.UseSqlServer(secret.Value));

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IScheduledFlightRepository, ScheduledFLightRepository>();
            builder.Services.AddScoped<IAirportRepository, AirportRepository>();
            builder.Services.AddScoped<IAirlineRepository, AirlineRepository>();
            builder.Services.AddScoped<IBookedFlightRepository, BookedFlightRepository>();
            builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
            builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddScoped<IFlightService, FlightService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IBookingService, BookingService>();
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

            builder.Services.AddHostedService<TimerService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddTransient<IAzureStorage, AzureStorage>();

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddHttpClient();
        }
    }
}