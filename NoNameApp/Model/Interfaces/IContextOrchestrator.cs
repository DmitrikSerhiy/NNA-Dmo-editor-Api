using System;
using System.Threading.Tasks;

namespace Model.Interfaces {
    public interface IContextOrchestrator : IDisposable {
        Task CommitChangesAsync();
        bool HasChanges();
    }
}
