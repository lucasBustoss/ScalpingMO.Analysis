using ScalpingMO.Analysis.Analysis.ConsolidateData.Data;
using ScalpingMO.Analysis.Analysis.ConsolidateData.Interfaces;
using ScalpingMO.Analysis.Analysis.ConsolidateData.Models;
using ScalpingMO.Analysis.Analysis.ConsolidateData.Services;

namespace ScalpingMO.Analysis.Analysis.ConsolidateData.Worker
{
    public class DataWorker
    {
        private readonly MongoDBService _mongoDB;

        public DataWorker(string connectionString, string analysisDatabaseName, string extractDatabaseName)
        {
            _mongoDB = new MongoDBService(connectionString, analysisDatabaseName, extractDatabaseName);    
        }

        public void Execute()
        {
            List<Fixture> fixtures = new List<Fixture>(); 
            List<FootballApiFixture> footballApiFixtures = _mongoDB.GetFootballApiFixturesToFill();
            List<BetfairFixture> betfairFixtures = _mongoDB.GetBetfairFixtures();
            List<WilliamHillFixture> williamHillFixtures = _mongoDB.GetWilliamHillFixtures();

            foreach (FootballApiFixture footballApiFixture in footballApiFixtures)
            {
                Fixture fixture = new Fixture(
                    footballApiFixture.Date, 
                    footballApiFixture.Status,
                    footballApiFixture.Id, 
                    footballApiFixture.HomeTeam.Name, 
                    footballApiFixture.HomeTeam.Logo, 
                    footballApiFixture.HomeTeam.Goals,
                    footballApiFixture.AwayTeam.Name, 
                    footballApiFixture.AwayTeam.Logo,
                    footballApiFixture.AwayTeam.Goals);

                #region Betfair

                List<BetfairFixture> betfairFixturesInSameDate = betfairFixtures.Where(b => b.Date == footballApiFixture.Date).ToList();
                List<IFixture> betfairFixturesAsIFixture = betfairFixturesInSameDate.Cast<IFixture>().ToList();
                BetfairFixture betfairFixtureWithCloserRatio = ComparationService.GetBestMatchFixture(footballApiFixture, betfairFixturesAsIFixture) as BetfairFixture;

                if (betfairFixtureWithCloserRatio != null)
                {
                    fixture.BetfairId = betfairFixtureWithCloserRatio.Id;
                    fixture.MarketId = betfairFixtureWithCloserRatio.MarketId;
                    fixture.BetfairHomeTeamId = betfairFixtureWithCloserRatio.HomeTeamId;
                    fixture.BetfairAwayTeamId = betfairFixtureWithCloserRatio.AwayTeamId;
                }

                #endregion

                #region William Hill

                List<WilliamHillFixture> williamHillFixturesInSameDate = williamHillFixtures.Where(b => b.Date == footballApiFixture.Date).ToList();
                List<IFixture> williamHillFixturesAsIFixture = williamHillFixturesInSameDate.Cast<IFixture>().ToList();
                WilliamHillFixture williamHillFixtureWithCloserRatio = ComparationService.GetBestMatchFixture(footballApiFixture, williamHillFixturesAsIFixture) as WilliamHillFixture;

                if (williamHillFixtureWithCloserRatio != null)
                {
                    fixture.WilliamHillId = williamHillFixtureWithCloserRatio.Id;
                    fixture.RadarUrl = williamHillFixtureWithCloserRatio.RadarUrl;
                }

                #endregion

                fixtures.Add(fixture);
            }

            _mongoDB.SaveFixtures(fixtures);
        }
    }
}
