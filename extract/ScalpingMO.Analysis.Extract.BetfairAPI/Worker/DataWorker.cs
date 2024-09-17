using ScalpingMO.Analysis.Extract.BetfairAPI.Data;
using ScalpingMO.Analysis.Extract.BetfairAPI.Models;

namespace ScalpingMO.Analysis.Extract.BetfairAPI.Worker
{
    public class DataWorker
    {
        private ApiService _api;
        private readonly MongoDBService _mongoDB;

        public DataWorker(BetfairConfiguration configuration)
        {
            _api = new ApiService(configuration);
            _mongoDB = new MongoDBService();
        }

        public void Execute()
        {
            List<Fixture> fixtures = _api.GetFixtures().GetAwaiter().GetResult();
            _mongoDB.SaveFixtures(fixtures);
        }
    }
}
