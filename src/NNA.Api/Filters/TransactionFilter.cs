using Microsoft.AspNetCore.Mvc.Filters;
using NNA.Domain.Interfaces;

namespace NNA.Api.Filters;

public class TransactionFilter : IAsyncActionFilter {
    private readonly IContextOrchestrator _contextOrchestrator;

    public TransactionFilter(IContextOrchestrator contextOrchestrator) {
        _contextOrchestrator = contextOrchestrator ?? throw new ArgumentNullException(nameof(contextOrchestrator));
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next) {
        //before each action
        var executedContext = await next();
        //after each action
        if (executedContext.Exception == null && _contextOrchestrator.HasChanges()) {
            await _contextOrchestrator.CommitChangesAsync();
            // todo: call _contextOrchestrator.dispose here? maybe I should use result filter here instead?
        }
    }
}