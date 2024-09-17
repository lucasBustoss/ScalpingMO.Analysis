using MongoDB.Driver;
using ScalpingMO.Analysis.Extract.FixtureData.Models;

namespace ScalpingMO.Analysis.Extract.FixtureData.Data
{
    public class MongoDBService
    {
        private readonly IMongoCollection<Fixture> _fixturesCollection;

        public MongoDBService()
        {
            var client = new MongoClient("mongodb://scalping:scalping_mo@localhost:27017/admin");
            var database = client.GetDatabase("scalping");
            _fixturesCollection = database.GetCollection<Fixture>("fixtures");
        }

        public List<Fixture> GetFixturesToExtractData()
        {
            DateTime actualDate = DateTime.UtcNow;
            List<Fixture> fixtures = 
                _fixturesCollection.Find(f => 
                    f.WilliamHillId != null && 
                    f.BetfairId != null && 
                    f.Status != "FT" &&
                    f.ShouldAnalyze && actualDate >= f.Date.AddMinutes(-1)
                ).ToList();
            
            return fixtures;
        }
    }
}
