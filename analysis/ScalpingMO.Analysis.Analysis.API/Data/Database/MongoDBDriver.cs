using MongoDB.Driver;
using ScalpingMO.Analysis.Analysis.API.Models.Users;

namespace ScalpingMO.Analysis.Analysis.API.Data.Database
{
    public class MongoDBDriver
    {
        public IMongoCollection<UserModel> UsersCollection { get; set; }

        public MongoDBDriver()
        {
            var client = new MongoClient("mongodb://scalping:scalping_mo@localhost:27017/admin");
            var database = client.GetDatabase("scalping");

            UsersCollection = database.GetCollection<UserModel>("users");
        }
    }
}
