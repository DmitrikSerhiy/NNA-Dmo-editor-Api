namespace NNA.Domain.Models;
public sealed class JwtOptions {
    public string Key { get; set; } = null!;
    public string SigningAlg { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public int TokenLifetimeInHours { get; set; }
}