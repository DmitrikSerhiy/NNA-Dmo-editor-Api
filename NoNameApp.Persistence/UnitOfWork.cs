using System;
using System.Threading.Tasks;

namespace NoNameApp.Persistence {
    public class UnitOfWork : IDisposable {

        private readonly NoNameAppContext _context;
        private Boolean _disposed;
        // ReSharper disable UnusedMember.Global
        protected UnitOfWork() { }
        public UnitOfWork(NoNameAppContext context) {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        internal NoNameAppContext Context {
            get {
                ThrowIfDisposed();
                return _context;
            }
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


        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~UnitOfWork() {
            Dispose(false);
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
    }
}
