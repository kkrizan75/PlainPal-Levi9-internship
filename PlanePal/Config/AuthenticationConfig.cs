using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PlanePal.Services.Shared;
using System.Text;

namespace PlanePal.Config
{
    public static class AuthenticationConfig
    {
        public static void ConfigureAuthentication(WebApplicationBuilder builder)
        {
            KeyVaultSecret jwtKey = AzureKeyVaultClientProvider.GetClient().GetSecret("Jwt--Key");

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.IncludeErrorDetails = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey.Value))
                    };
                });
        }
    }
}