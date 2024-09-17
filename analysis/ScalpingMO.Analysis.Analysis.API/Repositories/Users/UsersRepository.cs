using MongoDB.Bson;
using MongoDB.Driver;
using ScalpingMO.Analysis.Analysis.API.Data.Database;
using ScalpingMO.Analysis.Analysis.API.Models.Users;
using ScalpingMO.Analysis.Analysis.API.Models.Users.Response;

namespace ScalpingMO.Analysis.Analysis.API.Repositories.Users
{
    public class UsersRepository : IUsersRepository
    {
        private readonly IMongoCollection<UserModel> _usersCollection;

        public UsersRepository()
        {
            _usersCollection = new MongoDBDriver().UsersCollection;
        }

        public UserResponse Create(UserModel user)
        {
            try
            {
                user.Id = ObjectId.GenerateNewId();
                _usersCollection.InsertOne(user);

                return new UserResponse(user);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public UserResponse FindUserByUsername(string username)
        {
            UserModel user = _usersCollection.Find(f => f.Username == username).FirstOrDefault();

            if (user != null)
                return new UserResponse(user);

            return null;
        }
    }
}
