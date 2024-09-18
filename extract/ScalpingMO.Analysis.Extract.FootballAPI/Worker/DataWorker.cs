using ScalpingMO.Analysis.Extract.FootballAPI.Data;
using ScalpingMO.Analysis.Extract.FootballAPI.Models;
using ScalpingMO.Analysis.Extract.FootballAPI.Models.Fixture;
using ScalpingMO.Analysis.Extract.FootballAPI.Models.Response;

namespace ScalpingMO.Analysis.Extract.FootballAPI.Worker
{
    public class DataWorker
    {
        private ApiService _api;
        private readonly MongoDBService _mongoDB;

        public DataWorker(string url, string apiKey, string apiHost, string connectionString, string databaseName)
        {
            _api = new ApiService(url, apiKey, apiHost, connectionString, databaseName);
            _mongoDB = new MongoDBService(connectionString, databaseName);
        }

        public void Execute()
        {
            Sync sync = _mongoDB.GetSyncData();

            if (sync == null || sync.DateTime.AddSeconds(30) < DateTime.Now)
            {
                Console.WriteLine($"{DateTime.UtcNow} - Iniciando extração dos dados do FootballAPI");

                for (int i = 0; i < 7; i++)
                {
                    string date = DateTime.Now.AddDays(i).ToString("yyyy-MM-dd");
                    List<FixtureResponse> fixtures = _api.GetFixtures(date).GetAwaiter().GetResult();
                    TreatAndSaveFixtures(fixtures, date);
                }

                Console.WriteLine($"{DateTime.UtcNow} - Fim da extração dos dados do FootballAPI");
            } 
            else
                Console.WriteLine("Ainda não é possivel realizar o sync");
        }

        #region Private methods

        private void TreatAndSaveFixtures(List<FixtureResponse> fixtures, string date)
        {
            List<Fixture> newFixtures = new List<Fixture>();

            foreach (FixtureResponse fixture in fixtures)
            {
                Fixture newFixture = new Fixture(fixture.Fixture, fixture.Teams.HomeTeam, fixture.Teams.AwayTeam, fixture.Goals);
                newFixtures.Add(newFixture);
            }

            _mongoDB.SaveFixtures(newFixtures);
        }

        #endregion
    }
}
