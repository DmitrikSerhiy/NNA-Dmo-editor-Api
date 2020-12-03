using System;
using System.Threading.Tasks;

namespace Model.Interfaces {
    public interface IUnitOfWork : IDisposable {
        Task CommitChangesAsync();
        bool HasChanges();
    }
}
