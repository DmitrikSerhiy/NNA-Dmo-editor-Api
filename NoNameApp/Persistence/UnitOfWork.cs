using System;
using System.Threading.Tasks;

namespace Persistence {
    public class UnitOfWork : IDisposable {
        private readonly NoNameContext _context;
        private Boolean _disposed;
        public UnitOfWork() { }

        public UnitOfWork(NoNameContext context) {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

       


        public IDisposable BeginTransaction() {
            ThrowIfDisposed();
            if (_context.Database.CurrentTransaction != null) {
                throw new InvalidOperationException("Transaction is already started");
            }

            return _context.Database.BeginTransaction();
        }

        public async Task Commit() {
            ThrowIfDisposed();
            if (_context.Database.CurrentTransaction == null) {
                throw new InvalidOperationException("Transaction is not started. Commit failed");
            }

            await _context.SaveChangesAsync();
            _context.Database.CurrentTransaction.Commit();
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
