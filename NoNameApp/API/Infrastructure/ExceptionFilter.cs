using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using Serilog;

namespace API.Infrastructure {
    public class ExceptionFilter : IExceptionFilter {

        public void OnException(ExceptionContext context) {
            if (context.Exception == null) return;
            Log.Error("From exception filter: ", context.Exception);
            context.Result = new ObjectResult(new {error = "Internal server error", exception = context.Exception.Message}) {
                StatusCode = (int) HttpStatusCode.InternalServerError
            };
            context.ExceptionHandled = true;
        }
    }
}
