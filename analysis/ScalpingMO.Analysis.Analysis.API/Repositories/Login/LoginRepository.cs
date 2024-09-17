using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using ScalpingMO.Analysis.Analysis.API.Data.Database;
using ScalpingMO.Analysis.Analysis.API.Models.Login.Request;
using ScalpingMO.Analysis.Analysis.API.Models.Login.Response;
using ScalpingMO.Analysis.Analysis.API.Models.Users;
using ScalpingMO.Analysis.Analysis.API.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace ScalpingMO.Analysis.Analysis.API.Repositories.Login
{
    public class LoginRepository : ILoginRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IMongoCollection<UserModel> _usersCollection;

        public LoginRepository(IConfiguration configuration)
        {
            _usersCollection = new MongoDBDriver().UsersCollection;
            _configuration = configuration;
        }

        public LoginResponse Login(LoginRequest loginRequest)
        {
            UserModel user = FindUserByUsername(loginRequest.Username);

            if (user == null)
                throw new InvalidDataException("User or password is incorrect.");

            bool passwordIsValid = PasswordHelper.VerifyPassword(loginRequest.Password, user.Password);

            if (!passwordIsValid)
                throw new InvalidDataException("User or password is incorrect.");

            var key = Encoding.ASCII.GetBytes(_configuration["JwtBearerTokenSettings:SecretKey"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
                Audience = _configuration["JwtBearerTokenSettings:Audience"],
                Issuer = _configuration["JwtBearerTokenSettings:Issuer"],
                Expires = DateTime.UtcNow.AddHours(Convert.ToInt32(_configuration["JwtBearerTokenSettings:ExpireTimeInHours"]))
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenObject = tokenHandler.CreateToken(tokenDescriptor);

            var token = tokenHandler.WriteToken(tokenObject);

            return new LoginResponse(user, token);
        }

        #region Private methods

        private UserModel FindUserByUsername(string username)
        {
            UserModel user = _usersCollection.Find(f => f.Username == username).FirstOrDefault();

            if (user != null)
                return user;

            return null;
        }

        #endregion
    }
}
