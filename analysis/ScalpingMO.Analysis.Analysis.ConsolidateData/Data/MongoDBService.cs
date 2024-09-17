using MongoDB.Driver;
using ScalpingMO.Analysis.Analysis.ConsolidateData.Models;

namespace ScalpingMO.Analysis.Analysis.ConsolidateData.Data
{
    public class MongoDBService
    {
        private readonly IMongoCollection<Fixture> _fixturesCollection;
        private readonly IMongoCollection<FootballApiFixture> _footballApiFixtureCollection;
        private readonly IMongoCollection<BetfairFixture> _betfairFixtureCollection;
        private readonly IMongoCollection<WilliamHillFixture> _williamHillFixtureCollection;

        public MongoDBService()
        {
            var client = new MongoClient("mongodb://scalping:scalping_mo@localhost:27017/admin");
            var database = client.GetDatabase("scalping");
            _fixturesCollection = database.GetCollection<Fixture>("fixtures");
            _footballApiFixtureCollection = database.GetCollection<FootballApiFixture>("footballapi_fixtures");
            _betfairFixtureCollection = database.GetCollection<BetfairFixture>("betfair_fixtures");
            _williamHillFixtureCollection = database.GetCollection<WilliamHillFixture>("williamhill_fixtures");
        }

        public void SaveFixtures(List<Fixture> fixtures)
        {
            List<Fixture> fixturesToSave = new List<Fixture>();

            foreach (Fixture fixture in fixtures)
            {
                var filter = Builders<Fixture>.Filter.Eq(m => m.Id, fixture.Id);
                _fixturesCollection.ReplaceOne(filter, fixture, new ReplaceOptions { IsUpsert = true });
            }
        }

        public List<FootballApiFixture> GetFootballApiFixturesToFill()
        {
            // Filtra todos os documentos em 'footballApi_fixtures' que não existem em 'fixture'
            var filterNotInFixture = Builders<FootballApiFixture>.Filter.And(
                Builders<FootballApiFixture>.Filter.Not(Builders<FootballApiFixture>.Filter.In(
                    f => f.HomeTeam.Name,
                    _fixturesCollection.AsQueryable().Select(f => f.HomeTeamName)
                )),
                Builders<FootballApiFixture>.Filter.Not(Builders<FootballApiFixture>.Filter.In(
                    f => f.AwayTeam.Name,
                    _fixturesCollection.AsQueryable().Select(f => f.AwayTeamName)
                )),
                Builders<FootballApiFixture>.Filter.Not(Builders<FootballApiFixture>.Filter.In(
                    f => f.Date,
                    _fixturesCollection.AsQueryable().Select(f => f.Date)
                ))
            );

            // Filtra documentos que existem em 'fixture', mas possuem 'williamHillId' ou 'betfairId' nulos
            var filterWithNullIds = Builders<FootballApiFixture>.Filter.And(
                Builders<FootballApiFixture>.Filter.In(
                    f => f.HomeTeam.Name,
                    _fixturesCollection.AsQueryable().Where(f => f.WilliamHillId == null || f.BetfairId == null).Select(f => f.HomeTeamName)
                ),
                Builders<FootballApiFixture>.Filter.In(
                    f => f.AwayTeam.Name,
                    _fixturesCollection.AsQueryable().Where(f => f.WilliamHillId == null || f.BetfairId == null).Select(f => f.AwayTeamName)
                ),
                Builders<FootballApiFixture>.Filter.In(
                    f => f.Date,
                    _fixturesCollection.AsQueryable().Where(f => f.WilliamHillId == null || f.BetfairId == null).Select(f => f.Date)
                )
            );

            // Combina os dois filtros usando OR
            var combinedFilter = Builders<FootballApiFixture>.Filter.Or(filterNotInFixture, filterWithNullIds);

            // Executa a consulta
            return _footballApiFixtureCollection.Find(combinedFilter).ToList();
        }

        public List<FootballApiFixture> GetFootballApiFixtures()
        {
            return _footballApiFixtureCollection.Find(Builders<FootballApiFixture>.Filter.Empty, new FindOptions()).ToList();
        }

        public List<BetfairFixture> GetBetfairFixtures()
        {
            return _betfairFixtureCollection.Find(Builders<BetfairFixture>.Filter.Empty, new FindOptions()).ToList();
        }

        public List<WilliamHillFixture> GetWilliamHillFixtures()
        {
            return _williamHillFixtureCollection.Find(Builders<WilliamHillFixture>.Filter.Empty, new FindOptions()).ToList();
        }
    }
}
