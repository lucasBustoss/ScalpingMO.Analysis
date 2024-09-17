using ScalpingMO.Analysis.Extract.FixtureData.Data;
using ScalpingMO.Analysis.Extract.FixtureData.Models;
using ScalpingMO.Analysis.Extract.FixtureData.Worker;

namespace ScalpingMO.Analysis.Extract.FixtureData
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            MongoDBService mongoDb = new MongoDBService();
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
                            DataWorker worker = new DataWorker();
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
