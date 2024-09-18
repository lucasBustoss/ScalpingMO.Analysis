using ScalpingMO.Analysis.Extract.WilliamHill.Data;
using ScalpingMO.Analysis.Extract.WilliamHill.Models;

namespace ScalpingMO.Analysis.Extract.WilliamHill.Worker
{
    public class DataWorker
    {
        private ScrapperService _scrapper;
        private MongoDBService _mongoDB;

        public DataWorker(string url, string connectionString, string databaseName)
        {
            _scrapper = new ScrapperService(url);
            _mongoDB = new MongoDBService(connectionString, databaseName);
        }

        public void Execute()
        {
            List<Fixture> fixtures = _scrapper.GetFixtures();
            _mongoDB.SaveFixtures(fixtures);
        }
    }
}
