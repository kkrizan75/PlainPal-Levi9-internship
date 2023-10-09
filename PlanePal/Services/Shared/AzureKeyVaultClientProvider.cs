using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Serilog;

namespace PlanePal.Services.Shared
{
    public static class AzureKeyVaultClientProvider
    {
        private static SecretClient _client;

        public static SecretClient GetClient()
        {
            if (_client == null)
            {
                var kvURL = "https://kvappplanepaldev02.vault.azure.net/";
                if (string.IsNullOrWhiteSpace(kvURL))
                {
                    Log.Error("KVUrl not found in the .env file.");
                    return null;
                }
#if DEBUG
                _client = new SecretClient(new Uri(kvURL), new DefaultAzureCredential());
#else 
                _client = new SecretClient(new Uri(kvURL), new ManagedIdentityCredential());
#endif
            }
            return _client;
        }
    }
}