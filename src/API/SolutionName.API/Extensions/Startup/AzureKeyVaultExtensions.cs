using Azure.Identity;

namespace SolutionName.API.Extensions.Startup
{
    public class AzureKeyVaultExtensions
    {
        public static void UseAzureKeyVault(WebApplicationBuilder builder)
        {
            var vaultName = builder.Configuration["KeyVault:VaultName"];
            if (string.IsNullOrEmpty(vaultName))
                return;

            var vaultUri = new Uri($"https://{vaultName}.vault.azure.net/");
            builder.Configuration.AddAzureKeyVault(vaultUri, new DefaultAzureCredential());
        }
    }
}


