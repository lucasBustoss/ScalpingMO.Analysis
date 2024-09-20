using MongoDB.Bson;

namespace ScalpingMO.Analysis.Extract.FixtureData.Models.BetfairAPI.Scrapper
{
    public class BetfairCookie
    {
        public BetfairCookie(string accessToken, string refreshToken, string source, string exchange)
        {
            Id = ObjectId.GenerateNewId();
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            Source = source;
            Exchange = exchange;
        }

        public ObjectId Id { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string Source { get; set; }
        public string Exchange { get; set; }
    }
}
