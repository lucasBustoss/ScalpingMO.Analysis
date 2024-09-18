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

        public MongoDBService(string connectionString, string analysisDatabaseName, string extractDatabaseName)
        {
            var client = new MongoClient(connectionString);

            var analysisDatabase = client.GetDatabase(analysisDatabaseName);
            _fixturesCollection = analysisDatabase.GetCollection<Fixture>("fixtures");
            
            var extractDatabase = client.GetDatabase(extractDatabaseName);
            _footballApiFixtureCollection = extractDatabase.GetCollection<FootballApiFixture>("footballapi_fixtures");
            _betfairFixtureCollection = extractDatabase.GetCollection<BetfairFixture>("betfair_fixtures");
            _williamHillFixtureCollection = extractDatabase.GetCollection<WilliamHillFixture>("williamhill_fixtures");
        }

        public void SaveFixtures(List<Fixture> fixtures)
        {
            foreach (Fixture fixture in fixtures)
            {
                var filter = Builders<Fixture>.Filter.Eq(f => f.FootballApiId, fixture.FootballApiId);
                var existingFixture = _fixturesCollection.Find(filter).FirstOrDefault();

                if (existingFixture == null)
                    _fixturesCollection.InsertOne(fixture);
                else
                {
                    // Se o fixture já existir, atualiza apenas as propriedades necessárias
                    var update = Builders<Fixture>.Update
                        .Set(f => f.HomeTeamGoals, fixture.HomeTeamGoals)
                        .Set(f => f.AwayTeamGoals, fixture.AwayTeamGoals)
                        .Set(f => f.Status, fixture.Status)
                        .Set(f => f.BetfairId, fixture.BetfairId)
                        .Set(f => f.MarketId, fixture.MarketId)
                        .Set(f => f.WilliamHillId, fixture.WilliamHillId)
                        .Set(f => f.RadarUrl, fixture.RadarUrl);

                    _fixturesCollection.UpdateOne(filter, update, new UpdateOptions { IsUpsert = true });
                }
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
