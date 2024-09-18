using FuzzySharp;
using Microsoft.Extensions.Configuration;
using ScalpingMO.Analysis.Analysis.ConsolidateData.Worker;
using SimMetrics.Net.Metric;

namespace ScalpingMO.Analysis.Analysis.ConsolidateData
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

            string connectionString = config["MongoDB:ConnectionString"];
            string analysisDatabaseName = config["MongoDB:AnalysisDatabaseName"];
            string extractDatabaseName = config["MongoDB:ExtractDatabaseName"];

            DataWorker worker = new DataWorker(connectionString, analysisDatabaseName, extractDatabaseName);

            while (true)
            {
                int minute = 60000;

                Console.WriteLine($"Iniciando consolidação dos dados");
                worker.Execute();
                Console.WriteLine($"Fim da consolidação dos dados.");

                Thread.Sleep(minute * 10);
            }
        }
    }
}
