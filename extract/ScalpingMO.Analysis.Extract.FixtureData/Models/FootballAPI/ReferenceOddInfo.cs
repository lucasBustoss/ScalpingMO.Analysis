using MongoDB.Bson;
using ScalpingMO.Analysis.Extract.FixtureData.Models.FootballAPI.Response.Odds;
using System.Globalization;

namespace ScalpingMO.Analysis.Extract.FixtureData.Models.FootballAPI.Response
{
    public class ReferenceOddInfo
    {
        public ReferenceOddInfo(FixtureDataResponse fixture, List<OddsDataResponse> odds)
        {
            OddsDataResponse oddObject = odds.FirstOrDefault();

            Id = ObjectId.GenerateNewId();
            FootballApiId = fixture.Id;
            CreationDate = DateTime.UtcNow;
            Minute = fixture.Status.Seconds;

            OddsDataValueResponse homeOddObject = oddObject.Values.Find(v => v.Side == "Home");
            HomeOdd = Convert.ToDouble(homeOddObject.Odd, CultureInfo.InvariantCulture);

            OddsDataValueResponse drawOddObject = oddObject.Values.Find(v => v.Side == "Draw");
            DrawOdd = Convert.ToDouble(drawOddObject.Odd, CultureInfo.InvariantCulture);

            OddsDataValueResponse awayOddObject = oddObject.Values.Find(v => v.Side == "Away");
            AwayOdd = Convert.ToDouble(awayOddObject.Odd, CultureInfo.InvariantCulture);
        }

        public ObjectId Id { get; set; }
        public int FootballApiId { get; set; }
        public DateTime CreationDate { get; set; }
        public string Minute { get; set; }
        public double HomeOdd { get; set; }
        public double DrawOdd { get; set; }
        public double AwayOdd { get; set; }
    }
}
