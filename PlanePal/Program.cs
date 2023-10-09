using FluentValidation;
using FluentValidation.AspNetCore;
using PlanePal.AviationStackAPI;
using PlanePal.Config;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

ServiceConfig.AddServices(builder);

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.Services.AddEndpointsApiExplorer();

// Logging
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.ApplicationInsights(new Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration { InstrumentationKey = "28d1b4ab-1b93-4d23-b5fd-e879ab0c60a0" }, TelemetryConverter.Traces)
    .CreateLogger();

AuthenticationConfig.ConfigureAuthentication(builder);
SwaggerConfig.SetupSwagger(builder, Assembly.GetExecutingAssembly().GetName().Name);
ApiUtil.InitializeClient(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddAuthorization();

var app = builder.Build();

DatabaseConfig.InitializeDatabase(app);

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();