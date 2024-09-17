using MongoDB.Bson;
using ScalpingMO.Analysis.Analysis.API.Utils;

namespace ScalpingMO.Analysis.Analysis.API.Models.Users.Response
{
    public class UserResponse
    {
        public UserResponse(UserModel user)
        {
            Id = user.Id.ToString();
            Username = user.Username;
            Email = user.Email;
            Role = user.Role;
            PlanType = PlanHelpers.GetPlanType(user.PlanType);
            PlanExpiresAt = user.PlanExpiresAt;
        }

        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string PlanType { get; set; }
        public DateTime PlanExpiresAt { get; set; }
    }
}
