using ScalpingMO.Analysis.Analysis.API.Repositories.Users;

namespace ScalpingMO.Analysis.Analysis.API.Models.Users.Request
{
    public class UserCreateRequest: BaseModelRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public override List<string> Errors { get; protected set; }

        public override void Validate()
        {
            Errors = new List<string>();

            if (Username == null || Username == "") Errors.Add("Username cannot be empty or null");
            if (Email == null || Email == "") Errors.Add("Email cannot be empty or null");
        }

        public override bool IsValid()
        {
            Validate();

            return Errors.Count == 0;
        }

        public bool IsValidPassword()
        {
            if (Password == null || Password == "") Errors.Add("Password cannot be empty or null");

            return Errors.Count == 0;
        }

        public bool PasswordConfirmed()
        {
            if (Password != ConfirmPassword) Errors.Add("Both passwords should be equal");

            return Errors.Count == 0;
        }

        public bool ExistsUsername(IUsersRepository userRepository)
        {
            if (userRepository.FindUserByUsername(Username) != null) Errors.Add("Username already exists");

            return Errors.Count == 0;
        }
    }
}
