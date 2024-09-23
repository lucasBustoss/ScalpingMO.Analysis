using Microsoft.Extensions.Configuration;
using ScalpingMO.Analysis.Extract.FixtureData.Data;
using ScalpingMO.Analysis.Extract.FixtureData.Data.Betfair;
using ScalpingMO.Analysis.Extract.FixtureData.Models;
using ScalpingMO.Analysis.Extract.FixtureData.Worker;

namespace ScalpingMO.Analysis.Extract.FixtureData
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

            string betfairApiUrl = config["BetfairAPI:Url"];
            string betfairApiKey = config["BetfairAPI:AppKey"];
            string betfairApiToken = config["BetfairAPI:Token"];

            BetfairAPIService betfairApiService = new BetfairAPIService(betfairApiUrl, betfairApiKey, betfairApiToken);
            MongoDBService mongoDb = new MongoDBService(connectionString, analysisDatabaseName, extractDatabaseName);


            Console.WriteLine("Iniciando aplicação de verificação e extração de dados de jogos");

            string footballApiUrl = config["FootballAPI:Url"];
            string footballApiKey = config["FootballAPI:ApiKey"];
            string footballApiHost = config["FootballAPI:ApiHost"];

            FootballAPIService footballApiService = new FootballAPIService(mongoDb, footballApiUrl, footballApiKey, footballApiHost);
            HashSet<string> processedFixtures = new HashSet<string>();


            while (true)
            {
                Console.WriteLine($"{DateTime.UtcNow} - Consultando dados de odds de referência");
                await footballApiService.GetLiveOdds();
                Console.WriteLine($"{DateTime.UtcNow} - Fim da consulta de dados de odds de referência");

                List<Fixture> fixtures = mongoDb.GetFixturesToExtractData();
                List<Task> tasks = new List<Task>();

                foreach (Fixture fixture in fixtures)
                {
                    if (!processedFixtures.Contains(fixture.Id.ToString()))
                    {
                        // Dispara a execução do worker sem aguardar a conclusão
                        _ = Task.Run(async () =>
                        {
                            DataWorker worker = new DataWorker(mongoDb, betfairApiService);
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
