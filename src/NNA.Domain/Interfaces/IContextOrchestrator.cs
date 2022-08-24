using Microsoft.EntityFrameworkCore;

namespace NNA.Domain.Interfaces; 
public interface IContextOrchestrator : IDisposable {
    Task CommitChangesAsync();
    bool HasChanges();
    DbContext Context { get; }
}