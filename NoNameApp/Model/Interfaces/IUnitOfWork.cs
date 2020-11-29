using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Model.Interfaces
{
    public interface IUnitOfWork : IDisposable {
        Task CommitChangesAsync();
        bool HasChanges();
    }
}
