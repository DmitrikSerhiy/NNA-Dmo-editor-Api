namespace NNA.Domain.Interfaces; 
public interface IContextOrchestrator : IDisposable {
    Task CommitChangesAsync();
    bool HasChanges();
    string GetContextId();
}