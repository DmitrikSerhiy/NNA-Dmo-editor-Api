using Microsoft.EntityFrameworkCore;
using NNA.Domain.Entities;
using NNA.Domain.Interfaces;
using NNA.Domain.Interfaces.Repositories;

namespace NNA.Persistence.Repositories;

internal sealed class DmosRepository : IDmosRepository {
    private readonly NnaContext _context;

    public DmosRepository(IContextOrchestrator contextOrchestrator) {
        if (contextOrchestrator == null) throw new ArgumentNullException(nameof(contextOrchestrator));
        _context = (contextOrchestrator.Context as NnaContext)!;
    }

    public async Task<List<Dmo>> GetAllAsync(Guid userId, CancellationToken token) {
        return await _context.Dmos.Where(dmo => dmo.NnaUserId == userId)
            .OrderByDescending(dmo => dmo.DateOfCreation)
            .ToListAsync(token);
    }

    public async Task<Dmo?> GetShortDmoAsync(Guid userId, Guid? dmoId, CancellationToken token) {
        if (!dmoId.HasValue) throw new ArgumentNullException(nameof(dmoId));
        return await _context.Dmos.FirstOrDefaultAsync(d => d.NnaUserId == userId && d.Id == dmoId.Value, token);
    }

    public async Task<Dmo?> GetDmoAsync(Guid userId, Guid? dmoId, CancellationToken token) {
        if (!dmoId.HasValue) throw new ArgumentNullException(nameof(dmoId));
        return await _context.Dmos
            .Include(d => d.DmoCollectionDmos)
            .ThenInclude(dd => dd.DmoCollection)
            .Include(d => d.Beats)
            .FirstOrDefaultAsync(d => d.NnaUserId == userId && d.Id == dmoId.Value, token);
    }

    public void DeleteDmo(Dmo? dmo) {
        if (dmo == null) throw new ArgumentNullException(nameof(dmo));
        dmo.DmoCollectionDmos.Clear();
        _context.Beats.RemoveRange(dmo.Beats);
        _context.Dmos.Remove(dmo);
    }

    public async Task<List<Beat>> GetBeatsForDmoAsync(Guid userId, Guid dmoId, CancellationToken token) {
        return await _context.Beats
            .AsNoTracking()
            .Where(b => b.DmoId == dmoId && b.UserId == userId)
            .OrderBy(b => b.Order)
            .ToListAsync(token);
    }

    public string GetContextId() {
        return _context.ContextId.ToString();
    }
}