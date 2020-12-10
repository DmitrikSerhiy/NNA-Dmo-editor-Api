namespace Model.DTOs.Editor.Response {
    public class EditorResponseDto<T> : BaseEditorResponseDto where T : BaseDto {

        public new int HttpCode { get; private set; }
        public new string Header { get; private set; }
        public new bool IsSuccessful { get; private set; }
        public T Data { get; private set; }

        public static EditorResponseDto<T> Ok(T data) {
            var response = CreateSuccessfulResult();
            var responseWithData = new EditorResponseDto<T> {
                Data = data,
                Header = response.Header,
                HttpCode = response.HttpCode,
                IsSuccessful = response.IsSuccessful
            };
            return responseWithData;
        }
    }
}
