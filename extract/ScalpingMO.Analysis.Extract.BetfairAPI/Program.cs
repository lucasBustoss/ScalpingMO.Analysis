using Microsoft.Extensions.Configuration;
using ScalpingMO.Analysis.Extract.BetfairAPI.Models;
using ScalpingMO.Analysis.Extract.BetfairAPI.Worker;

namespace ScalpingMO.Analysis.Extract.BetfairAPI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())  // Diretório base onde o appsettings está
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)  // Carrega o appsettings.json
               .Build();

            string username = config["Betfair:Username"];
            string password = config["Betfair:Password"];
            string apiKey = config["Betfair:ApiKey"];
            string secretTwoFactor = config["Betfair:SecretTwoFactorCode"];

            BetfairConfiguration betfairConfig = new BetfairConfiguration(username, password, apiKey, secretTwoFactor);

            DataWorker worker = new DataWorker(betfairConfig);

            while (true)
            {
                Console.WriteLine($"{DateTime.UtcNow} - Iniciando extração dos dados da Betfair");
                worker.Execute();
                Console.WriteLine($"{DateTime.UtcNow} - Fim da extração dos dados da Betfair");
                Thread.Sleep(20000);
            }
        }
    }
}
