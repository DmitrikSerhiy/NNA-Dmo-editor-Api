using Microsoft.AspNetCore.Http;

namespace Model.DTOs.Editor
{
    public class EditorErrorDetailsDto : BaseDto {
        public EditorErrorDetailsDto(string errorMessage) {
            ErrorMessage = errorMessage;
            Header = "Error";
        }

        public EditorErrorDetailsDto CreateInternalServerError() {
            Header = $"{StatusCodes.Status500InternalServerError} {Header}";
            return this;
        }

        public string Header { get; set; }
        public string ErrorMessage { get; set; }
    }
}
