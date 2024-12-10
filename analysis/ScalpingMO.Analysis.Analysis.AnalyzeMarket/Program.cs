using Microsoft.Extensions.Configuration;
using ScalpingMO.Analysis.Analysis.AnalyzeMarket.Data;
using ScalpingMO.Analysis.Analysis.AnalyzeMarket.Models;
using ScalpingMO.Analysis.Analysis.AnalyzeMarket.Worker;

namespace ScalpingMO.Analysis.Analysis.AnalyzeMarket
{
    internal class Program
    {
        static async Task Main(string[] args)
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

            string previousMinutesToAnalyze = config["Parameters:PreviousMinutesToAnalyze"];

            MongoDBService mongoDB = new MongoDBService(connectionString, analysisDatabaseName, extractDatabaseName, Convert.ToInt32(previousMinutesToAnalyze));
            HashSet<string> processedFixtures = new HashSet<string>();

            while (true)
            {
                List<FixtureInfo> fixtures = mongoDB.GetFixturesToAnalyze();
                List<Task> tasks = new List<Task>();

                foreach (FixtureInfo fixture in fixtures)
                {
                    if (!processedFixtures.Contains(fixture.Id.ToString()))
                    {
                        // Dispara a execução do worker sem aguardar a conclusão
                        _ = Task.Run(async () =>
                        {
                            DataWorker worker = new DataWorker(mongoDB);
                            await worker.Execute(fixture);
                        });

                        processedFixtures.Add(fixture.Id.ToString());
                    }
                }

                await Task.Delay(30000);
            }

        }
    }
}
