using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helpers.GlobalFilters {
    public class ValidationFilter : IAsyncActionFilter {
        
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next) {
            if (!context.ModelState.IsValid) {
                context.Result = new ObjectResult(
                    new NnaValidationResult {
                        Fields = context.ModelState
                            .Select(modelState => new NnaValidationResultFields {
                                Field = modelState.Key,
                                Errors = modelState.Value.Errors.Select(er => er.ErrorMessage).ToArray()
                            })
                            .ToArray()
                    }) {
                    StatusCode = (int)HttpStatusCode.UnprocessableEntity
                };
                return;
            }
            await next();
        }
        
    }
}