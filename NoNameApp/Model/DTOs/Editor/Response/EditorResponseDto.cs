namespace Model.DTOs.Editor.Response {
    public class EditorResponseDto<T> : BaseEditorResponseDto where T : BaseDto {

        public EditorResponseDto(T result) {
            Result = result;
            CreateSuccessfulResult();
        }

        public T Result { get; }
    }
}
