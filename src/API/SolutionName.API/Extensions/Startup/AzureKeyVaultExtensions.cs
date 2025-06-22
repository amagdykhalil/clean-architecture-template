using Azure.Identity;

namespace SolutionName.API.Extensions.Startup
{
    public static class AzureKeyVaultExtensions
    {
        public static void ConfigureAzureKeyVault(this WebApplicationBuilder builder)
        {
            var vaultName = builder.Configuration["KeyVault:VaultName"];
            if (string.IsNullOrEmpty(vaultName))
                return;

            var vaultUri = new Uri($"https://{vaultName}.vault.azure.net/");
            builder.Configuration.AddAzureKeyVault(vaultUri, new DefaultAzureCredential());
        }
    }
}


