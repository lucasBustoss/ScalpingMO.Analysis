using Microsoft.Extensions.Configuration;
using ScalpingMO.Analysis.Extract.FootballAPI.Worker;

namespace ScalpingMO.Analysis.Extract.FootballAPI
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

            string url = config["FootballAPI:Url"];
            string apiKey = config["FootballAPI:ApiKey"];
            string apiHost = config["FootballAPI:ApiHost"];

            string connectionString = config["MongoDB:ConnectionString"];
            string databaseName = config["MongoDB:DatabaseName"];

            DataWorker worker = new DataWorker(url, apiKey, apiHost, connectionString, databaseName);

            while (true)
            {
                int minute = 60000;
                Console.WriteLine("Checando se é possivel realizar um novo sync do FootballAPI");
                worker.Execute();

                Thread.Sleep(minute * 5);
            }
        }
    }
}
