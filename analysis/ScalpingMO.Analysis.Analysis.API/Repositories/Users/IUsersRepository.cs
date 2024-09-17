using ScalpingMO.Analysis.Analysis.API.Models.Users.Response;
using ScalpingMO.Analysis.Analysis.API.Models.Users;

namespace ScalpingMO.Analysis.Analysis.API.Repositories.Users
{
    public interface IUsersRepository
    {
        UserResponse Create(UserModel user);
        UserResponse FindUserByUsername(string username);
    }
}
