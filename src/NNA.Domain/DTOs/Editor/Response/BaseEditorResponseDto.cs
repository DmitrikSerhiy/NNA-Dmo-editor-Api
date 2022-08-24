using System.Net;

namespace NNA.Domain.DTOs.Editor.Response; 
public class BaseEditorResponseDto : BaseDto {

    // ReSharper disable InconsistentNaming
    public int httpCode { get; private set; }
    public string header { get; private set; }
    public string message { get; private set; }
    public bool isSuccessful { get; private set; }

    public List<EditorErrorDetailsDto> errors { get; private set; } = new ();
    public List<EditorValidationDetailsDto> warnings { get; private set; } = new ();


    public static BaseEditorResponseDto CreateInternalServerErrorResponse(string errorMessage) {
        return new BaseEditorResponseDto {
            errors = new List<EditorErrorDetailsDto> { new (errorMessage) },
            httpCode = (int)HttpStatusCode.InternalServerError,
            header = "Error",
            message = "Internal Server Error",
            isSuccessful = false
        };
    }

    public static BaseEditorResponseDto CreateFailedValidationResponse(List<Tuple<string, string>> validationDetails) {
        return new BaseEditorResponseDto {
            warnings = validationDetails.Select(v => new EditorValidationDetailsDto(v.Item1, v.Item2)).ToList(),
            httpCode = (int)HttpStatusCode.UnprocessableEntity,
            header = "Warning",
            message = "Validation failed",
            isSuccessful = false
        };
    }

    public static BaseEditorResponseDto CreateFailedAuthResponse() {
        return new BaseEditorResponseDto {
            errors = new List<EditorErrorDetailsDto> { new ("Hub context does not contain auth token") },
            httpCode = (int)HttpStatusCode.Unauthorized,
            header = "Error",
            message = "User is not authorized",
            isSuccessful = false
        };
    }

    public static BaseEditorResponseDto CreateBadRequestResponse() {
        return new BaseEditorResponseDto {
            httpCode = (int)HttpStatusCode.BadRequest, 
            header = "Bad request",
            isSuccessful = false
        };
    }

    public static BaseEditorResponseDto CreateNoContentResponse() {
        return new BaseEditorResponseDto {
            httpCode = (int)HttpStatusCode.NoContent, 
            header = "Ok", 
            isSuccessful = true
        };
    }

    public static BaseEditorResponseDto CreateSuccessfulResult() {
        return new BaseEditorResponseDto {
            httpCode = (int)HttpStatusCode.OK, 
            header = "Ok",
            isSuccessful = true
        };
    }
}