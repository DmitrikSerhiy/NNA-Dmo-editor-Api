using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Serilog;

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

        public async Task CommitChangesAsync() {
            ThrowIfDisposed();
            if (_context.Database.CurrentTransaction != null) {
                throw new InvalidOperationException("Transaction is already started");
            }

            await _context.Database.CreateExecutionStrategy().ExecuteAsync(async () => {
                IDbContextTransaction transaction = null;
                try {
                    using (transaction = await _context.Database.BeginTransactionAsync()) {
                        await _context.SaveChangesAsync();
                        await _context.Database.CurrentTransaction.CommitAsync();
                    }
                }
                catch (DbUpdateException ex) {
                    if (transaction != null) {
                        await transaction.RollbackAsync();
                    }
                    Log.Error("Transaction failed", ex);
                    throw;
                }
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
