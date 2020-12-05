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

        public List<EditorErrorDetailsDto> Errors { get; } = new List<EditorErrorDetailsDto>();
        public List<EditorValidationDetailsDto> Warnings { get; } = new List<EditorValidationDetailsDto>();


        public BaseEditorResponseDto CreateInternalServerErrorResponse(string errorMessage) {
            Errors.Add(new EditorErrorDetailsDto(errorMessage));
            HttpCode = StatusCodes.Status500InternalServerError;
            Header = "Error";
            Message = "Internal Server Error";
            IsSuccessful = false;
            return this;
        }

        public BaseEditorResponseDto CreateFailedValidationResponse(List<Tuple<string, string>> validationDetails) {
            Warnings.AddRange(validationDetails.Select(v => new EditorValidationDetailsDto(v.Item1, v.Item2)));
            HttpCode = StatusCodes.Status422UnprocessableEntity;
            Header = "Warning";
            Message = "Validation failed";
            IsSuccessful = false;
            return this;
        }

        public BaseEditorResponseDto CreateFailedAuthResponse() {
            Errors.Add(new EditorErrorDetailsDto("Hub context does not contain auth token"));
            HttpCode = StatusCodes.Status401Unauthorized;
            Header = "Error";
            Message = "User is not authorized";
            IsSuccessful = false;
            return this;
        }

        public BaseEditorResponseDto CreateNoContentResponse() {
            HttpCode = StatusCodes.Status204NoContent;
            Header = "Ok";
            IsSuccessful = true;
            return this;
        }

        protected BaseEditorResponseDto CreateSuccessfulResult() {
            HttpCode = StatusCodes.Status200OK;
            Header = "Ok";
            IsSuccessful = true;
            return this;
        }


    }
}
