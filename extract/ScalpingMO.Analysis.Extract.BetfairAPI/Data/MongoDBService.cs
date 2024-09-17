using MongoDB.Driver;
using ScalpingMO.Analysis.Extract.BetfairAPI.Models;

namespace ScalpingMO.Analysis.Extract.BetfairAPI.Data
{
    public class MongoDBService
    {
        private readonly IMongoCollection<Sync> _syncCollection;
        private readonly IMongoCollection<Fixture> _fixtureCollection;


        public MongoDBService()
        {
            var client = new MongoClient("mongodb://scalping:scalping_mo@localhost:27017/admin");
            var database = client.GetDatabase("scalping");
            _syncCollection = database.GetCollection<Sync>("betfair_sync");
            _fixtureCollection = database.GetCollection<Fixture>("betfair_fixtures");
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

            UpsertSync();
        }

        #region Private methods

        private void UpsertSync()
        {
            var update = Builders<Sync>.Update
                .Set(m => m.DateTime, DateTime.Now);

            _syncCollection.UpdateOne(Builders<Sync>.Filter.Empty, update, new UpdateOptions { IsUpsert = true });
        }

        #endregion
    }
}
