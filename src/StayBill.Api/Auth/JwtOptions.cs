namespace StayBill.Api.Auth;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = "StayBill";
    public string Audience { get; set; } = "StayBill";
    public int ExpiresMinutes { get; set; } = 480;
}
