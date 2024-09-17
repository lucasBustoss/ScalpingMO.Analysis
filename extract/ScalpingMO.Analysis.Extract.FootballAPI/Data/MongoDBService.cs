using MongoDB.Driver;
using ScalpingMO.Analysis.Extract.FootballAPI.Models;
using ScalpingMO.Analysis.Extract.FootballAPI.Models.Fixture;

namespace ScalpingMO.Analysis.Extract.FootballAPI.Data
{
    public class MongoDBService
    {
        private readonly IMongoCollection<Sync> _syncCollection;
        private readonly IMongoCollection<RateLimit> _rateCollection;
        private readonly IMongoCollection<Fixture> _fixtureCollection;

        public MongoDBService()
        {
            var client = new MongoClient("mongodb://scalping:scalping_mo@localhost:27017/admin");
            var database = client.GetDatabase("scalping");
            _syncCollection = database.GetCollection<Sync>("footballapi_sync");
            _rateCollection = database.GetCollection<RateLimit>("footballapi_rate");
            _fixtureCollection = database.GetCollection<Fixture>("footballapi_fixtures");
        }

        public void SaveFixtures(List<Fixture> fixtures)
        {
            List<Fixture> fixturesToSave = new List<Fixture>();

            foreach (Fixture fixture in fixtures)
            {
                Fixture existentfixture = fixturesToSave.Where(m => m.Id == fixture.Id).FirstOrDefault();
                Fixture existentfixtureInDb = _fixtureCollection.Find(m => m.Id == fixture.Id).FirstOrDefault();

                if (existentfixture == null && existentfixtureInDb == null)
                    fixturesToSave.Add(fixture);
            }

            if (fixturesToSave.Count > 0)
                _fixtureCollection.InsertMany(fixturesToSave);

            UpsertSync("fixture");
        }

        public RateLimit GetRateLimit()
        {
            RateLimit rate = _rateCollection.Find(Builders<RateLimit>.Filter.Empty).FirstOrDefault();
            return rate;
        }

        public void UpdateRateLimit(RateLimit rateLimit)
        {
            var update = Builders<RateLimit>.Update
                .Set(m => m.RequestsLimit, rateLimit.RequestsLimit)
                .Set(m => m.RequestsRemaining, rateLimit.RequestsRemaining)
                .Set(m => m.RequestsReset, rateLimit.RequestsReset)
                .Set(m => m.LastRequest , rateLimit.LastRequest);

            _rateCollection.UpdateOne(Builders<RateLimit>.Filter.Empty, update, new UpdateOptions { IsUpsert = true });
        }

        #region Private methods

        private void UpsertSync(string method)
        {
            var filter = Builders<Sync>.Filter.Eq(m => m.Method, method);

            var update = Builders<Sync>.Update
                .Set(m => m.DateTime, DateTime.Now);

            _syncCollection.UpdateOne(filter, update, new UpdateOptions { IsUpsert = true });
        }

        #endregion
    }
}
