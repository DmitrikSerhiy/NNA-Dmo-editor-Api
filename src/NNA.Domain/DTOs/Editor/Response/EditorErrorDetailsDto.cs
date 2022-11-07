namespace NNA.Domain.DTOs.Editor.Response;

public sealed class EditorErrorDetailsDto : BaseDto {
    public EditorErrorDetailsDto(string errorMessage) {
        this.errorMessage = errorMessage;
    }

    // ReSharper disable once InconsistentNaming
    public string errorMessage { get; }
}