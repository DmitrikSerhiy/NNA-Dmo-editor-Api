using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Model.Interfaces;

namespace API.Helpers.GlobalFilters
{
    public class TransactionFilter : IAsyncActionFilter {
        private readonly IUnitOfWork _unitOfWork;
        public TransactionFilter(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next) {
            //if (!context.ModelState.IsValid) {
            //    context.Result = new BadRequestObjectResult(
            //        context.ModelState);
            //    return;
            //    //todo: Disable automatic 400 response and add some custom global invalidModel response
            //}

            //before each action
            var executedContext = await next();
            //after each action
            if (executedContext.Exception == null && _unitOfWork.HasChanges())
            {
                await _unitOfWork.CommitChangesAsync();
            }
        }
    }
}
