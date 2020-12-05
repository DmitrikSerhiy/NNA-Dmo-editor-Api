namespace Model.DTOs.Editor.Response {
    public class EditorResponseDto<T> : BaseEditorResponseDto where T : BaseDto {

        public EditorResponseDto(T data) {
            Data = data;
            CreateSuccessfulResult();
        }

        public T Data { get; }
    }
}
