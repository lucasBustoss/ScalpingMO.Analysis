namespace ScalpingMO.Analysis.Extract.BetfairAPI.Models
{
    public class BetfairConfiguration
    {
        public BetfairConfiguration(string username, string password, string apiKey, string secretTwoFactorCode)
        {
            Username = username;
            Password = password;
            ApiKey = apiKey;
            SecretTwoFactorCode = secretTwoFactorCode;
        }

        public string Username { get; set; }
        public string Password { get; set; }
        public string ApiKey { get; set; }
        public string SecretTwoFactorCode { get; set; }
        public string Token { get; set; }
    }
}
