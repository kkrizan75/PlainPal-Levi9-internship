using Azure.Security.KeyVault.Secrets;
using PlanePal.Services.Shared;
using System.Net.Http.Headers;

namespace PlanePal.AviationStackAPI
{
    public static class ApiUtil
    {
        public static IConfiguration Configuration { get; private set; }
        public static HttpClient ApiClient { get; set; }

        public static void InitializeClient(IConfiguration configuration)
        {
            ApiClient = new HttpClient();
            ApiClient.DefaultRequestHeaders.Accept.Clear();
            ApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Configuration = configuration;
        }

        public static string GetUrl(string dataType, Dictionary<string, string> optionalParams)
        {
            var keyVaultClient = AzureKeyVaultClientProvider.GetClient();
            KeyVaultSecret secret = keyVaultClient.GetSecret("FlightApiKey");

            var secretValue = secret.Value;
            var url = $"{Configuration["BaseUrl"]}{dataType}?access_key=" + secretValue;
            if (optionalParams is not null)
            {
                foreach (var option in optionalParams)
                {
                    url += $"&{option.Key}={option.Value}";
                }
            }
            return url;
        }
    }
}