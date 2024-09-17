namespace ScalpingMO.Analysis.Extract.WilliamHill.Models
{
    public class Fixture
    {
        public Fixture()
        {
            
        }

        public Fixture(int id, string homeTeam, string awayTeam, DateTime date, string hour)
        {
            Id = id;
            HomeTeam = homeTeam;
            AwayTeam = awayTeam;
            
            DateTime parsedHour = DateTime.ParseExact(hour, "HH:mm", System.Globalization.CultureInfo.InvariantCulture);
            Date = date.Date.Add(parsedHour.TimeOfDay);

            RadarUrl = $"https://sports.whcdn.net/scoreboards/app/football/index.html?eventId={id}";
        }

        public int Id { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public DateTime Date { get; set; }
        public string RadarUrl { get; set; }
    }
}
