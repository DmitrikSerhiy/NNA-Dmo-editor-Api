using System.Net;
using System.Security.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;

namespace NNA.Api.Filters;

public class ExceptionFilter : IAsyncExceptionFilter {
    public Task OnExceptionAsync(ExceptionContext context) {
        Log.Error(context.Exception, $"From exception filter: {context.Exception.Message}");
        ObjectResult result;
        if (context.Exception is AuthenticationException) {
            result = new ObjectResult(new {
                fromExceptionFilter = true,
                title = "Authentication error",
                code = (int)HttpStatusCode.Unauthorized,
                message = context.Exception.Message,
                exception = context.Exception.StackTrace
            }) {
                StatusCode = (int)HttpStatusCode.Unauthorized
            };
        }
        else {
            result = new ObjectResult(new {
                fromExceptionFilter = true,
                title = "Internal server error",
                code = (int)HttpStatusCode.InternalServerError,
                message = context.Exception.Message,
                exception = context.Exception.StackTrace
            }) {
                StatusCode = (int)HttpStatusCode.InternalServerError
            };
        }

        context.Result = result;
        context.ExceptionHandled = true;
        return Task.CompletedTask;
    }
}