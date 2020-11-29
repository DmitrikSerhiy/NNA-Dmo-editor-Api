using System.Collections.Generic;

namespace Model.DTOs.Editor
{
    public class BaseEditorResponseDto<T> where T : BaseDto {
        public BaseEditorResponseDto() { }

        public BaseEditorResponseDto(T result) {
            Result = result;
        }

        public BaseEditorResponseDto<T> AttachErrorDetails(EditorErrorDetailsDto errorDetails) {
            Errors.Add(errorDetails);
            return this;
        }

        public List<EditorErrorDetailsDto> Errors { get; set; } = new List<EditorErrorDetailsDto>();

        public T Result { get; set; }
    }
}
