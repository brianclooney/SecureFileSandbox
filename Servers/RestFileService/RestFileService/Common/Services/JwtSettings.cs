namespace RestFileService.Common.Services;

public class JwtSettings
{
    public static string SectionName { get; } = "JwtSettings";
    public string Secret { get; init; } = default!;
    public string Issuer { get; init; } = default!;
    public string Audience { get; init; } = default!;
    public int LifeSpanMinutes { get; init; }
}
