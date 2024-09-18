using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using ScalpingMO.Analysis.Extract.BetfairAPI.Models;

namespace ScalpingMO.Analysis.Extract.BetfairAPI.Data
{
    public class MongoDBService
    {
        private readonly IMongoCollection<Fixture> _fixtureCollection;

        public MongoDBService(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);

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
        }
    }
}
