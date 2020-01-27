using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Persistence {
    public class UnitOfWork : IDisposable {
        private readonly NoNameContext _context;
        private Boolean _disposed;
        public UnitOfWork() { }

        public UnitOfWork(NoNameContext context) {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        internal NoNameContext Context {
            get {
                ThrowIfDisposed();
                return _context;
            }
        }

        public void CommitChanges() {
            ThrowIfDisposed();
            if (_context.Database.CurrentTransaction != null) {
                throw new InvalidOperationException("Transaction is already started");
            }

            var strategy = _context.Database.CreateExecutionStrategy();
            strategy.Execute(() => {
                _context.Database.BeginTransaction();
                _context.SaveChanges();
                _context.Database.CurrentTransaction.Commit();
            });
        }

        public Boolean HasChanges() {
            return _context.ChangeTracker.HasChanges();
        }

        private void ThrowIfDisposed() {
            if (_disposed) {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        private void Dispose(Boolean disposing) {
            if (_disposed) {
                return;
            }

            if (disposing) {
                _context.Dispose();
            }
            _disposed = true;
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~UnitOfWork() {
            Dispose(false);
        }
    }
}
