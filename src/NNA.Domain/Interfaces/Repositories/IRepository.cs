namespace NNA.Domain.Interfaces.Repositories;

public interface IRepository {
    string GetContextId();
    Task SyncContextImmediatelyAsync(CancellationToken token);
}