using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Model.DTOs.Editor.Response {
    public class BaseEditorResponseDto : BaseDto {

        public int HttpCode { get; private set; }
        public string Header { get; private set; }
        public string Message { get; private set; }
        public bool IsSuccessful { get; private set; }

        public List<EditorErrorDetailsDto> Errors { get; private set; } = new List<EditorErrorDetailsDto>();
        public List<EditorValidationDetailsDto> Warnings { get; private set; } = new List<EditorValidationDetailsDto>();


        public static BaseEditorResponseDto CreateInternalServerErrorResponse(string errorMessage) {
            return new BaseEditorResponseDto {
                Errors = new List<EditorErrorDetailsDto> {new EditorErrorDetailsDto(errorMessage)},
                HttpCode = StatusCodes.Status500InternalServerError,
                Header = "Error",
                Message = "Internal Server Error",
                IsSuccessful = false
            };
        }

        public static BaseEditorResponseDto CreateFailedValidationResponse(List<Tuple<string, string>> validationDetails) {
            return new BaseEditorResponseDto {
                Warnings = validationDetails.Select(v => new EditorValidationDetailsDto(v.Item1, v.Item2)).ToList(),
                HttpCode = StatusCodes.Status422UnprocessableEntity,
                Header = "Warning",
                Message = "Validation failed",
                IsSuccessful = false
            };
        }

        public static BaseEditorResponseDto CreateFailedAuthResponse() {
            return new BaseEditorResponseDto {
                Errors = new List<EditorErrorDetailsDto>
                    {new EditorErrorDetailsDto("Hub context does not contain auth token")},
                HttpCode = StatusCodes.Status401Unauthorized,
                Header = "Error",
                Message = "User is not authorized",
                IsSuccessful = false
            };
        }

        public static BaseEditorResponseDto CreateBadRequestResponse() {
            return new BaseEditorResponseDto {
                HttpCode = StatusCodes.Status400BadRequest, 
                Header = "Bad request",
                IsSuccessful = false
            };
        }

        public static BaseEditorResponseDto CreateNoContentResponse() {
            return new BaseEditorResponseDto {
                HttpCode = StatusCodes.Status204NoContent, 
                Header = "Ok", 
                IsSuccessful = true
            };
        }

        protected static BaseEditorResponseDto CreateSuccessfulResult() {
            return new BaseEditorResponseDto {
                HttpCode = StatusCodes.Status200OK, 
                Header = "Ok",
                IsSuccessful = true
            };
        }
    }
}
