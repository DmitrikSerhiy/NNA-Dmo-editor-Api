using System.Net;
using Microsoft.AspNetCore.Mvc;
using NNA.Api.Helpers;

namespace NNA.Api.Features;

public class NnaController : ControllerBase {
    public IActionResult OkWithData(object data) {
        return new JsonResult(data);
    }

    public IActionResult BadRequestWithMessageForUi(string message) {
        return StatusCode((int)HttpStatusCode.BadRequest,
            ResponseBuilder.AppendBadRequestErrorMessageToForm(message));
    }

    public IActionResult BadRequestWithMessageToToastr(string message) {
        return StatusCode((int)HttpStatusCode.BadRequest,
            ResponseBuilder.AppendBadRequestErrorMessage(message));
    }

    public IActionResult InvalidRequestWithValidationMessagesToToastr(string fieldName, string message) {
        return StatusCode((int)HttpStatusCode.UnprocessableEntity,
            ResponseBuilder.AppendValidationErrorMessage(fieldName, message));
    }
}