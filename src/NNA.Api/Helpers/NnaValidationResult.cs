namespace NNA.Api.Helpers;

public sealed class NnaValidationResult {
    public string Title { get; set; } = "Server validation failed";
    public NnaValidationResultFields[] Fields { get; set; } = Array.Empty<NnaValidationResultFields>();
}