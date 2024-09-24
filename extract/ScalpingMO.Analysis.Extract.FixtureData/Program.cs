using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;
using ScalpingMO.Analysis.Extract.FixtureData.Data;
using ScalpingMO.Analysis.Extract.FixtureData.Data.Betfair;
using ScalpingMO.Analysis.Extract.FixtureData.Models;
using ScalpingMO.Analysis.Extract.FixtureData.Worker;

namespace ScalpingMO.Analysis.Extract.FixtureData
{
    internal class Program
    {
        private static IPage _page;
        private static IBrowser _browser;

        static async Task Main(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            await StartDriverAsync();

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
                await footballApiService.GetLiveOdds();

                List<Fixture> fixtures = mongoDb.GetFixturesToExtractData();
                List<Task> tasks = new List<Task>();

                foreach (Fixture fixture in fixtures)
                {
                    if (!processedFixtures.Contains(fixture.Id.ToString()))
                    {
                        // Dispara a execução do worker sem aguardar a conclusão
                        _ = Task.Run(async () =>
                        {
                            DataWorker worker = new DataWorker(mongoDb, betfairApiService, _browser);
                            await worker.Execute(fixture);
                        });

                        processedFixtures.Add(fixture.Id.ToString());
                    }
                }

                await Task.Delay(30000);
            }

        }

        private static async Task StartDriverAsync()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var playwright = await Playwright.CreateAsync();
            var browserOptions = new BrowserTypeLaunchOptions();

            if (environment == "Docker")
            {
                browserOptions.Headless = true;
                browserOptions.Args = new[]
                {
                    "--no-sandbox",
                    "--disable-dev-shm-usage",
                    "--disable-gpu",
                    "--disable-software-rasterizer",
                    "--disable-extensions",
                    "--disable-infobars",
                    "--disable-blink-features=AutomationControlled",
                    "--window-size=1920,1080"
                };
            }

            _browser = await playwright.Chromium.LaunchAsync(browserOptions);
        }
    }
}
