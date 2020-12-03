namespace Model.DTOs.Editor.Response
{
    public class EditorErrorDetailsDto : BaseDto {
        public EditorErrorDetailsDto(string errorMessage) {
            ErrorMessage = errorMessage;
        }

        public string ErrorMessage { get; }
    }
}
