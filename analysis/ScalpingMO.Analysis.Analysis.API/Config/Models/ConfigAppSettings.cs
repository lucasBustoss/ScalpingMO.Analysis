namespace ScalpingMO.Analysis.Analysis.API.Config.Models
{
    public class ConfigAppSettings
    {
        public string SecretKey { get; set; }
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public int ExpireTimeInHours { get; set; }
    }
}
