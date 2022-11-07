namespace NNA.Domain.DTOs.Editor.Response;

public sealed class EditorValidationDetailsDto : BaseDto {
    public EditorValidationDetailsDto(string validationMessage, string fieldName) {
        ValidationMessage = validationMessage;
        FieldName = fieldName;
    }

    public string ValidationMessage { get; }
    public string FieldName { get; }
}