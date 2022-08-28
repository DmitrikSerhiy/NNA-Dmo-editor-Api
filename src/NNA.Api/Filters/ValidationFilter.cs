using System.Net;
using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NNA.Api.Helpers;
using NNA.Domain.DTOs;

namespace NNA.Api.Filters;
public class ValidationFilter : IAsyncActionFilter {
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var dtoToValidate = context.ActionArguments
            .FirstOrDefault(arg => arg.Value?.GetType().IsSubclassOf(typeof(BaseDto)) == true)
            .Value;

        if (dtoToValidate is not null) {
            await ValidateAsync(context, dtoToValidate);
            if (!context.ModelState.IsValid) {
                context.Result = new ObjectResult(
                    new NnaValidationResult {
                        Fields = context.ModelState
                            .Select(modelState => new NnaValidationResultFields {
                                Field = modelState.Key,
                                Errors = modelState.Value!.Errors.Select(er => er.ErrorMessage).ToArray()
                            })
                            .ToArray()
                    }) {
                    StatusCode = (int)HttpStatusCode.UnprocessableEntity
                };
                return;
            }
        }

        await next();
    }


    private async Task ValidateAsync(ActionExecutingContext context, object dtoToValidate)
    {
        var dtoType = dtoToValidate.GetType();
        var validatorType = typeof(IValidator<>).MakeGenericType(dtoType);
        var validator = context.HttpContext.RequestServices.GetService(validatorType);
        if (validator is null) 
        {
            throw new Exception($"Failed to resolve validator for {dtoType.Name} dto");
        }
        
        var validationResultTask = (Task<ValidationResult>)validatorType.GetMethod("ValidateAsync")!.Invoke(validator, new []{dtoToValidate, CancellationToken.None})!;
        var validationResult = await validationResultTask;
        validationResult.AddToModelState(context.ModelState, "");
    }
}
