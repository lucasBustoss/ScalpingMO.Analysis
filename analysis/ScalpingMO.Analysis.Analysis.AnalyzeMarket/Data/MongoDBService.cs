using MongoDB.Driver;
using ScalpingMO.Analysis.Analysis.AnalyzeMarket.Models;
using ScalpingMO.Analysis.Analysis.AnalyzeMarket.Models.Betfair;
using ScalpingMO.Analysis.Analysis.AnalyzeMarket.Models.FootballAPI;
using ScalpingMO.Analysis.Analysis.AnalyzeMarket.Models.WilliamHill;

namespace ScalpingMO.Analysis.Analysis.AnalyzeMarket.Data
{
    public class MongoDBService
    {
        private readonly int _previousMinutesToAnalyze;
        private readonly IMongoCollection<FixtureInfo> _fixturesCollection;
        private readonly IMongoCollection<FixtureMarketAnalyze> _marketCollection;
        private readonly IMongoCollection<BetfairInfo> _betfairFixturesCollection;
        private readonly IMongoCollection<FootballAPIInfo> _footballAPIFixturesCollection;
        private readonly IMongoCollection<WilliamHillInfo> _williamHillFixturesCollection;

        public MongoDBService(string connectionString, string analysisDatabaseName, string extractDatabaseName, int previousMinutesToAnalyze)
        {
            var client = new MongoClient(connectionString);

            var analysisDatabase = client.GetDatabase(analysisDatabaseName);
            _fixturesCollection = analysisDatabase.GetCollection<FixtureInfo>("fixtures");
            _marketCollection = analysisDatabase.GetCollection<FixtureMarketAnalyze>("fixture_market");

            var extractDatabase = client.GetDatabase(extractDatabaseName);
            _betfairFixturesCollection = extractDatabase.GetCollection<BetfairInfo>("betfair_scrapper_info");
            _footballAPIFixturesCollection = extractDatabase.GetCollection<FootballAPIInfo>("footballapi_info");
            _williamHillFixturesCollection = extractDatabase.GetCollection<WilliamHillInfo>("williamhill_info");
            _previousMinutesToAnalyze = previousMinutesToAnalyze;
        }

        public List<FixtureInfo> GetFixturesToAnalyze()
        {
            DateTime actualDate = DateTime.UtcNow;
            List<FixtureInfo> fixtures =
                _fixturesCollection.Find(f =>
                    f.WilliamHillId != null &&
                    f.BetfairId != null &&
                    f.Status != "FT" &&
                    f.ShouldAnalyze && 
                    actualDate >= f.Date.AddMinutes(-1)
                ).ToList();

            return fixtures;
        }

        public List<BetfairInfo> GetBetfairInfoToAnalyze(int eventId)
        {
            DateTime actualDate = DateTime.UtcNow;
            List<BetfairInfo> fixtures =
                _betfairFixturesCollection.Find(f =>
                    f.EventId == eventId &&
                    actualDate >= f.DateTime.AddMinutes(_previousMinutesToAnalyze)
                ).SortByDescending(f => f.DateTime).Limit(5).ToList();

            return fixtures;
        }

        public List<FootballAPIInfo> GetFootballAPIInfoToAnalyze(int fixtureId)
        {
            DateTime actualDate = DateTime.UtcNow;
            List<FootballAPIInfo> fixtures =
                _footballAPIFixturesCollection.Find(f =>
                    f.FootballApiId == fixtureId &&
                    actualDate >= f.CreationDate.AddMinutes(_previousMinutesToAnalyze)
                ).ToList();

            return fixtures;
        }

        public List<WilliamHillInfo> GetWilliamHillInfoToAnalyze(int fixtureId)
        {
            DateTime actualDate = DateTime.UtcNow;
            List<WilliamHillInfo> fixtures =
                _williamHillFixturesCollection.Find(f =>
                    f.WilliamHillId == fixtureId &&
                    actualDate >= f.CreationDate.AddMinutes(_previousMinutesToAnalyze)
                ).ToList();

            return fixtures;
        }
    }
}
