namespace Model.DTOs.Editor.Response; 
public class EditorErrorDetailsDto : BaseDto {
    public EditorErrorDetailsDto(string errorMessage) {
        this.errorMessage = errorMessage;
    }

    // ReSharper disable once InconsistentNaming
    public string errorMessage { get; }
}