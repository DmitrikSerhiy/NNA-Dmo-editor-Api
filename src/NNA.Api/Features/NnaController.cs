using System.Net;
using Microsoft.AspNetCore.Mvc;
using NNA.Api.Helpers;

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
    protected IActionResult InvalidRequestWithValidationMessagesToToastr(string fieldName, string message) {
        return StatusCode((int)HttpStatusCode.UnprocessableEntity,
            ResponseBuilder.AppendValidationErrorMessage(fieldName, message));
    }
}