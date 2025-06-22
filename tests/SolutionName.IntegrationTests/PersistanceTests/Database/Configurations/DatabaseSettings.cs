namespace SolutionName.IntegrationTests.PersistanceTests.Database.Configurations
{
    public class DatabaseSettings
    {
        public int Port { get; set; }
        public string Password { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;

        public string GetConnectionString()
        {
            return $"Server=localhost,{Port};Database={DatabaseName};User Id=sa;Password={Password};TrustServerCertificate=True;";
        }
    }
} 