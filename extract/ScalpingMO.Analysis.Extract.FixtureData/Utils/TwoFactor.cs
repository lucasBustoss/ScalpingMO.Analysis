using TwoFactorAuthNet;

public static class TwoFactor
{
    public static string GenerateTwoFactorAuthCode(string secretKey)
    {
        var tfa = new TwoFactorAuth();
        string code = tfa.GetCode(secretKey);
        return code;
    }
}