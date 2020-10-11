using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Threading.Tasks;
using Serilog;

namespace API.Infrastructure {
    public class ExceptionFilter : IAsyncExceptionFilter {

        public Task OnExceptionAsync(ExceptionContext context) {
            if (context.Exception == null) return Task.CompletedTask;
            Log.Error(context.Exception, $"From exception filter: {context.Exception.Message}");
            context.Result = new ObjectResult(new
                {fromExceptionFilter = true, title = "Internal server error", code = (int)HttpStatusCode.InternalServerError, message = context.Exception.Message}) {
                StatusCode = (int) HttpStatusCode.InternalServerError
            };
            context.ExceptionHandled = true;
            return Task.CompletedTask;
        }
    }
}
