using System.Net;
using Microsoft.AspNetCore.Mvc;
using NNA.Api.Helpers;

namespace NNA.Api.Features;
public class NnaController : ControllerBase {

    private readonly ResponseBuilder _responseBuilder;

    public NnaController(ResponseBuilder responseBuilder) {
        _responseBuilder = responseBuilder ?? throw new ArgumentNullException(nameof(responseBuilder));
    }

    public IActionResult OkWithData(object data) {
        return new JsonResult(data);
    }

    public IActionResult BadRequestWithMessageForUi(string message) {
        return StatusCode((int)HttpStatusCode.BadRequest,
            _responseBuilder.AppendBadRequestErrorMessageToForm(message));
    }

    public IActionResult BadRequestWithMessageToToastr(string message) {
        return StatusCode((int)HttpStatusCode.BadRequest,
            _responseBuilder.AppendBadRequestErrorMessage(message));
    }

    public IActionResult InvalidRequestWithValidationMessagesToToastr(string fieldName, string message) {
        return StatusCode((int)HttpStatusCode.UnprocessableEntity,
            _responseBuilder.AppendValidationErrorMessage(fieldName, message));
    }
}
