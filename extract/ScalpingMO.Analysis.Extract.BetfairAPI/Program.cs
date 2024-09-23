using Microsoft.Extensions.Configuration;
using ScalpingMO.Analysis.Extract.BetfairAPI.Models;
using ScalpingMO.Analysis.Extract.BetfairAPI.Worker;

namespace ScalpingMO.Analysis.Extract.BetfairAPI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory) // Garante que o caminho seja correto no Docker
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .Build();

            string username = config["Betfair:Username"];
            string password = config["Betfair:Password"];
            string apiKey = config["Betfair:ApiKey"];
            string secretTwoFactor = config["Betfair:SecretTwoFactorCode"];

            string connectionString = config["MongoDB:ConnectionString"];
            string databaseName = config["MongoDB:DatabaseName"];

            BetfairConfiguration betfairConfig = new BetfairConfiguration(username, password, apiKey, secretTwoFactor);

            DataWorker worker = new DataWorker(betfairConfig, connectionString, databaseName);

            while (true)
            {
                int minute = 60000;

                try
                {
                    Console.WriteLine($"{DateTime.UtcNow} - Iniciando extração dos dados da Betfair");
                    worker.Execute();
                    Console.WriteLine($"{DateTime.UtcNow} - Fim da extração dos dados da Betfair");
                }
                catch
                {
                    Console.WriteLine($"{DateTime.UtcNow} - Erro na extração dos dados da Betfair");
                }

                Thread.Sleep(minute * 30);
            }
        }
    }
}
