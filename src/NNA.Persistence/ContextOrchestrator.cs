using Microsoft.EntityFrameworkCore;
using NNA.Domain.Interfaces;
using Serilog;

namespace NNA.Persistence;
// Unit of work
public class ContextOrchestrator : IContextOrchestrator {
    private NnaContext? _context;
    private bool _disposed;
    public ContextOrchestrator() { }

    public ContextOrchestrator(NnaContext context) {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public DbContext Context {
        get {
            ThrowIfDisposed();
            return _context!;
        }
    }

    public async Task CommitChangesAsync() {
        ThrowIfDisposed();
        if (_context!.Database.CurrentTransaction != null) {
            throw new InvalidOperationException("Transaction is already started");
        }

        await _context.Database.CreateExecutionStrategy().ExecuteAsync(async () => {
            try {
                await using var transaction = await _context.Database.BeginTransactionAsync();
                await _context.SaveChangesAsync();
                await _context.Database.CurrentTransaction!.CommitAsync();
            }
            catch (DbUpdateException ex) {
                Log.Error(ex, "Transaction failed");
                throw;
            }
        });
    }

    public bool HasChanges() {
        return _context!.ChangeTracker.HasChanges();
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
            _context!.Dispose();
            _context = null;
        }
        _disposed = true;
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~ContextOrchestrator() {
        Dispose(false);
    }
}