namespace Model.DTOs.Editor.Response {
    public class EditorValidationDetailsDto : BaseDto {

        public EditorValidationDetailsDto(string validationMessage, string fieldName) {
            ValidationMessage = validationMessage;
            FieldName = fieldName;
        }

        public string ValidationMessage { get; }
        public string FieldName { get; }
    }
}
