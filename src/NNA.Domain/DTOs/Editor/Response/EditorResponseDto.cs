namespace NNA.Domain.DTOs.Editor.Response;

public class EditorResponseDto<T> : BaseEditorResponseDto where T : BaseDto {
    // ReSharper disable InconsistentNaming
    public new int httpCode { get; private set; }
    public new string header { get; private set; } = null!;
    public new bool isSuccessful { get; private set; }
    public T data { get; private set; } = null!;

    public static EditorResponseDto<T> Ok(T data) {
        var response = CreateSuccessfulResult();
        var responseWithData = new EditorResponseDto<T> {
            data = data,
            header = response.header,
            httpCode = response.httpCode,
            isSuccessful = response.isSuccessful
        };
        return responseWithData;
    }
}