using ScalpingMO.Analysis.Analysis.API.Models.Login.Request;
using ScalpingMO.Analysis.Analysis.API.Models.Login.Response;

namespace ScalpingMO.Analysis.Analysis.API.Repositories.Login
{
    public interface ILoginRepository
    {
        LoginResponse Login(LoginRequest loginRequest);
    }
}
