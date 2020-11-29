using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Model.Interfaces;
using Serilog;

namespace Persistence {
    public class UnitOfWork : IUnitOfWork {
        private readonly NnaContext _context;
        private bool _disposed;
        // ReSharper disable once UnusedMember.Global
        public UnitOfWork() { }

        // ReSharper disable once UnusedMember.Global
        public UnitOfWork(NnaContext context) {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        internal NnaContext Context {
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
                try {
                    await using var transaction = await _context.Database.BeginTransactionAsync();
                    await _context.SaveChangesAsync();
                    await _context.Database.CurrentTransaction.CommitAsync();
                }
                catch (DbUpdateException ex) {
                    Log.Error(ex, "Transaction failed");
                    throw;
                }
            });
        }

        public bool HasChanges() {
            return _context.ChangeTracker.HasChanges();
        }

        private void ThrowIfDisposed() {
            if (_disposed) {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        private void Dispose(bool disposing) {
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
