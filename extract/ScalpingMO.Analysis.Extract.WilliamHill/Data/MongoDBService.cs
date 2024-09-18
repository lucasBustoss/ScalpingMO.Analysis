using MongoDB.Driver;
using ScalpingMO.Analysis.Extract.WilliamHill.Models;

namespace ScalpingMO.Analysis.Extract.WilliamHill.Data
{
    public class MongoDBService
    {
        private readonly IMongoCollection<Fixture> _fixtureCollection;

        public MongoDBService(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _fixtureCollection = database.GetCollection<Fixture>("williamhill_fixtures");
        }

        internal void SaveFixtures(List<Fixture> fixtures)
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
