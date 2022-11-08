using NNA.Domain.Interfaces;
using NNA.Domain.Interfaces.Repositories;

namespace NNA.Persistence.Repositories;

public class CommonRepository : IRepository {

    protected readonly NnaContext Context;

    public CommonRepository(IContextOrchestrator contextOrchestrator) {
        if (contextOrchestrator == null) throw new ArgumentNullException(nameof(contextOrchestrator));
        Context = (contextOrchestrator.Context as NnaContext)!;
    }

    public string GetContextId() {
        return Context.ContextId.ToString();
    }

    public async Task SyncContextImmediatelyAsync(CancellationToken token) {
        await Context.SaveChangesAsync(token);
    }
}
