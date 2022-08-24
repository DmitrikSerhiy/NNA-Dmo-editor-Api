namespace NNA.Domain.Models;
public sealed class JwtOptions {
    public string Key { get; set; }
    public string SigningAlg { get; set; }
    public string Audience { get; set; }
    public string Issuer { get; set; }
    public int TokenLifetimeInHours { get; set; }
}