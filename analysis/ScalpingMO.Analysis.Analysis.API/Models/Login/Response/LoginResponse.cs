using MongoDB.Bson;
using ScalpingMO.Analysis.Analysis.API.Models.Users;
using ScalpingMO.Analysis.Analysis.API.Utils;

namespace ScalpingMO.Analysis.Analysis.API.Models.Login.Response
{
    public class LoginResponse
    {
        public LoginResponse(UserModel user, string token)
        {
            Id = user.Id.ToString();
            Username = user.Username;
            Email = user.Email;
            Token = token;
            Role = user.Role;
            PlanType = PlanHelpers.GetPlanType(user.PlanType);
            PlanExpiresAt = user.PlanExpiresAt;
        }

        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
        public string PlanType { get; set; }
        public DateTime PlanExpiresAt { get; set; }
    }
}
