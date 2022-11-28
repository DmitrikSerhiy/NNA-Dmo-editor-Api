using System.Net;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using NNA.Api.Helpers;
using NuGet.Packaging;

namespace NNA.Api.Features;

public class NnaController : ControllerBase {
    [NonAction]
    protected IActionResult OkWithData(object data) {
        return new JsonResult(data);
    }
    
    [NonAction]
    protected IActionResult BadRequestWithMessageForUi(string message) {
        return StatusCode((int)HttpStatusCode.BadRequest,
            ResponseBuilder.AppendBadRequestErrorMessageToForm(message));
    }

    [NonAction]
    protected IActionResult BadRequestWithMessageToToastr(string message) {
        return StatusCode((int)HttpStatusCode.BadRequest,
            ResponseBuilder.AppendBadRequestErrorMessage(message));
    }

    [NonAction]
    protected IActionResult InvalidRequest(List<ValidationFailure> errors) {
        var result = new NnaValidationResult {
            Fields = errors
                .Select(error => new NnaValidationResultFields {
                    Field = error.PropertyName,
                    Errors = new[] { error.ErrorMessage }
                })
                .ToArray()
        };

        return StatusCode((int)HttpStatusCode.UnprocessableEntity, result);
    }
    
    [NonAction]
    protected IActionResult InvalidRequestWithValidationMessagesToToastr(string fieldName, string message) {
        return StatusCode((int)HttpStatusCode.UnprocessableEntity,
            ResponseBuilder.AppendValidationErrorMessage(fieldName, message));
    }
}