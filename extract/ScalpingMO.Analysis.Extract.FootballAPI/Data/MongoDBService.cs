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

        public MongoDBService(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _syncCollection = database.GetCollection<Sync>("footballapi_sync");
            _rateCollection = database.GetCollection<RateLimit>("footballapi_rate");
            _fixtureCollection = database.GetCollection<Fixture>("footballapi_fixtures");
        }

        public void SaveFixtures(List<Fixture> fixtures)
        {
            foreach (Fixture fixture in fixtures)
            {
                var filter = Builders<Fixture>.Filter.Eq(f => f.Id, fixture.Id);
                var existingFixture = _fixtureCollection.Find(filter).FirstOrDefault();

                if (existingFixture == null)
                    _fixtureCollection.InsertOne(fixture);
                else
                {
                    // Se o fixture já existir, atualiza apenas as propriedades necessárias
                    var update = Builders<Fixture>.Update
                        .Set(f => f.HomeTeam.Goals, fixture.HomeTeam.Goals)
                        .Set(f => f.AwayTeam.Goals, fixture.AwayTeam.Goals)
                        .Set(f => f.Status, fixture.Status);

                    _fixtureCollection.UpdateOne(filter, update, new UpdateOptions { IsUpsert = true });
                }
            }

            UpsertSync("fixture");
        }

        public RateLimit GetRateLimit()
        {
            RateLimit rate = _rateCollection.Find(Builders<RateLimit>.Filter.Empty).FirstOrDefault();
            return rate;
        }

        public Sync GetSyncData()
        {
            Sync sync = _syncCollection.Find(Builders<Sync>.Filter.Empty).FirstOrDefault();
            return sync;
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
