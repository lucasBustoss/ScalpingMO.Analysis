using ScalpingMO.Analysis.Analysis.ConsolidateData.Interfaces;
using ScalpingMO.Analysis.Analysis.ConsolidateData.Models;
using SimMetrics.Net.Metric;

namespace ScalpingMO.Analysis.Analysis.ConsolidateData.Services
{
    public static class ComparationService
    {
        private static double _ratio = 0.88;
        private static JaroWinkler _comparator = new JaroWinkler();
        
        public static IFixture GetBestMatchFixture(FootballApiFixture footballApiFixture, List<IFixture> otherSourceFixtures)
        {
            Dictionary<IFixture, double> fixtures = new Dictionary<IFixture, double>();

            foreach (IFixture fixture in otherSourceFixtures)
            {
                double homeTeamComparator = StringComparator(footballApiFixture.HomeTeam.Name, fixture.HomeTeam);
                double awayTeamComparator = StringComparator(footballApiFixture.AwayTeam.Name, fixture.AwayTeam);
                double dateComparator = DateComparator(footballApiFixture.Date, fixture.Date);

                double averageComparator = (double)(homeTeamComparator + awayTeamComparator + dateComparator) / 3;

                if (averageComparator >= _ratio)
                    fixtures.Add(fixture, averageComparator);
            }

            KeyValuePair<IFixture, double> bestMatch = fixtures.OrderByDescending(f => f.Value).FirstOrDefault();

            if (!bestMatch.Equals(default(KeyValuePair<IFixture, double>)))
                return bestMatch.Key;

            return null;
        }

        #region Private methods

        private static double StringComparator(string name1, string name2)
        {
            return _comparator.GetSimilarity(name1, name2);
        }

        private static double DateComparator(DateTime date1, DateTime date2)
        {
            return date1.Equals(date2) ? 1 : 0;
        }

        #endregion
    }
}
