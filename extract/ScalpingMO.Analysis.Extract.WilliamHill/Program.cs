using Microsoft.Extensions.Configuration;
using ScalpingMO.Analysis.Extract.WilliamHill.Worker;
using System;

namespace ScalpingMO.Analysis.Extract.WilliamHill
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

            string url = config["WilliamHill:Url"];

            string connectionString = config["MongoDB:ConnectionString"];
            string databaseName = config["MongoDB:DatabaseName"];

            DataWorker worker = new DataWorker(url, connectionString, databaseName);

            while (true)
            {
                int minute = 60000;

                Console.WriteLine($"{DateTime.UtcNow} - Iniciando extração dos dados da William Hill");
                worker.Execute();
                Console.WriteLine($"{DateTime.UtcNow} - Fim da extração dos dados da William Hill");

                Thread.Sleep(minute * 60);
            }
        }
    }
}
