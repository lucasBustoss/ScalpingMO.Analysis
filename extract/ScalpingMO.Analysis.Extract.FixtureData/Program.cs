using Microsoft.Extensions.Configuration;
using ScalpingMO.Analysis.Extract.FixtureData.Data;
using ScalpingMO.Analysis.Extract.FixtureData.Models;
using ScalpingMO.Analysis.Extract.FixtureData.Worker;

namespace ScalpingMO.Analysis.Extract.FixtureData
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Iniciando aplicação de verificação e extração de dados de jogos");
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory) // Garante que o caminho seja correto no Docker
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .Build();

            string connectionString = config["MongoDB:ConnectionString"];
            string databaseName = config["MongoDB:DatabaseName"];

            MongoDBService mongoDb = new MongoDBService(connectionString, databaseName);
            HashSet<string> processedFixtures = new HashSet<string>(); 

            while (true)
            {
                List<Fixture> fixtures = mongoDb.GetFixturesToExtractData();
                List<Task> tasks = new List<Task>();

                foreach (Fixture fixture in fixtures)
                {
                    if (!processedFixtures.Contains(fixture.Id.ToString()))
                    {
                        tasks.Add(Task.Run(() =>
                        {
                            DataWorker worker = new DataWorker(mongoDb);
                            worker.Execute(fixture);
                        }));

                        processedFixtures.Add(fixture.Id.ToString());
                    }
                }

                await Task.WhenAll(tasks);

                await Task.Delay(30000);
            }
        }
    }
}
