using MongoDB.Driver;
using ScalpingMO.Analysis.Extract.FixtureData.Models;
using ScalpingMO.Analysis.Extract.FixtureData.Models.FootballAPI;
using ScalpingMO.Analysis.Extract.FixtureData.Models.FootballAPI.Response;
using ScalpingMO.Analysis.Extract.FixtureData.Models.FootballAPI.Response.Odds;
using ScalpingMO.Analysis.Extract.FixtureData.Models.Radar;
using System.Text.Json;

namespace ScalpingMO.Analysis.Extract.FixtureData.Data
{
    public class MongoDBService
    {
        private readonly IMongoCollection<Fixture> _fixturesCollection;
        private readonly IMongoCollection<RadarInfo> _radarCollection;
        private readonly IMongoCollection<ReferenceOddInfo> _referenceOddCollection;
        private readonly IMongoCollection<RateLimit> _rateCollection;
        private readonly IMongoCollection<OddResponseRaw> _oddResponseRawCollection;

        public MongoDBService(string connectionString, string analysisDatabaseName, string extractDatabaseName)
        {
            var client = new MongoClient(connectionString);

            var analysisDatabase = client.GetDatabase(analysisDatabaseName);
            _fixturesCollection = analysisDatabase.GetCollection<Fixture>("fixtures");

            var extractDatabase = client.GetDatabase(extractDatabaseName);
            _radarCollection = extractDatabase.GetCollection<RadarInfo>("williamhill_info");
            _referenceOddCollection = extractDatabase.GetCollection<ReferenceOddInfo>("footballapi_info");
            _rateCollection = extractDatabase.GetCollection<RateLimit>("footballapi_rate");
            _oddResponseRawCollection = extractDatabase.GetCollection<OddResponseRaw>("footballapi_odd_raw");
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

        public void SaveRadarInfo(RadarInfo radarInfo)
        {
            RadarInfo existsInfo = _radarCollection.Find(f =>
                f.WilliamHillId == radarInfo.WilliamHillId &&
                f.Minute == radarInfo.Minute &&
                f.Description == radarInfo.Description).FirstOrDefault();

            if (existsInfo == null)
                _radarCollection.InsertOne(radarInfo);
        }

        #region FootballAPI

        public OddsResponse GetFixtureOdd(int fixtureId)
        {
            List<OddsResponse> odds = GetOddsResponseFromRaw();

            return odds.Where(o => o.Fixture.Id == fixtureId).FirstOrDefault();
        }

        public void SaveOddResponseRaw(string oddResponseRaw)
        {
            var update = Builders<OddResponseRaw>.Update.Set(f => f.Content, oddResponseRaw).Set(f => f.ExtractionDate, DateTime.UtcNow);
            _oddResponseRawCollection.UpdateOne(Builders<OddResponseRaw>.Filter.Empty, update, new UpdateOptions { IsUpsert = true });
        }

        public void SaveReferenceOddInfo(ReferenceOddInfo referenceOddInfo)
        {
            ReferenceOddInfo existsInfo = _referenceOddCollection.Find(f =>
                f.FootballApiId == referenceOddInfo.FootballApiId &&
                f.Minute == referenceOddInfo.Minute &&
                f.HomeOdd == referenceOddInfo.HomeOdd &&
                f.DrawOdd == referenceOddInfo.DrawOdd &&
                f.AwayOdd == referenceOddInfo.AwayOdd).FirstOrDefault();

            if (existsInfo == null)
                _referenceOddCollection.InsertOne(referenceOddInfo);
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
                .Set(m => m.LastRequest, rateLimit.LastRequest);

            _rateCollection.UpdateOne(Builders<RateLimit>.Filter.Empty, update, new UpdateOptions { IsUpsert = true });
        }

        #endregion

        #region Private methods

        #region FootballAPI

        private List<OddsResponse> GetOddsResponseFromRaw()
        {
            string oddsRaw = _oddResponseRawCollection.Find(Builders<OddResponseRaw>.Filter.Empty).FirstOrDefault().Content;
            ApiBaseResponse<OddsResponse> oddsObject = JsonSerializer.Deserialize<ApiBaseResponse<OddsResponse>>(oddsRaw);

            return oddsObject.Response.ToList();
        }

        #endregion

        #endregion
    }
}
