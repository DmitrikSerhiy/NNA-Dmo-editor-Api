namespace NNA.Api.Helpers;

public sealed class NnaValidationResultFields {
    public string Field { get; set; } = "";
    public string[] Errors { get; set; } = Array.Empty<string>();
}