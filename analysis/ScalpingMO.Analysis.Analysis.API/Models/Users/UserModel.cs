using ScalpingMO.Analysis.Analysis.API.Models.Users.Request;
using ScalpingMO.Analysis.Analysis.API.Utils;

namespace ScalpingMO.Analysis.Analysis.API.Models.Users
{
    public class UserModel : BaseModel
    {
        public UserModel()
        {
            
        }

        public UserModel(UserCreateRequest userCreateRequest)
        {
            Username = userCreateRequest.Username;
            Email = userCreateRequest.Email;
            Password = PasswordHelper.HashPassword(userCreateRequest.Password);
            Role = "user";
            PlanType = 3;
            PlanExpiresAt = DateTime.UtcNow.AddDays(7);
        }

        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public int PlanType { get; set; }
        public DateTime PlanExpiresAt { get; set; }
    }
}
