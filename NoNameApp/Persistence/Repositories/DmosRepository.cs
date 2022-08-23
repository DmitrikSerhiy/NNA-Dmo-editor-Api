using Microsoft.EntityFrameworkCore;
using Model.Entities;
using Model.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Persistence.Repositories;
internal sealed class DmosRepository : IDmosRepository {
    private readonly NnaContext _context;
    public DmosRepository(ContextOrchestrator contextOrchestrator) {
        if (contextOrchestrator == null) throw new ArgumentNullException(nameof(contextOrchestrator));

        _context = contextOrchestrator.Context;
        Console.WriteLine($"From DmosRepository {GetContextId()}");
    }

    public async Task<List<Dmo>> GetAll(Guid userId) {
        return await _context.Dmos.Where(d => d.NnaUserId == userId)
            .OrderByDescending(d => d.DateOfCreation)
            .ToListAsync();
    }

    public async Task<Dmo> GetShortDmo(Guid userId, Guid? dmoId) {
        if (!dmoId.HasValue) throw new ArgumentNullException(nameof(dmoId));
        return await _context.Dmos.FirstOrDefaultAsync(d => d.NnaUserId == userId && d.Id == dmoId.Value);
    }


    public async Task<Dmo> GetDmo(Guid userId, Guid? dmoId) {
        if (!dmoId.HasValue) throw new ArgumentNullException(nameof(dmoId));
        return await _context.Dmos
            .Include(d => d.DmoCollectionDmos)
            .ThenInclude(dd => dd.DmoCollection)
            .Include(d => d.Beats)
            .FirstOrDefaultAsync(d => d.NnaUserId == userId && d.Id == dmoId.Value);
    }

    public void DeleteDmo(Dmo dmo) {
        if (dmo == null) throw new ArgumentNullException(nameof(dmo));
        dmo.DmoCollectionDmos.Clear();
        _context.Beats.RemoveRange(dmo.Beats);
        _context.Dmos.Remove(dmo);
    }

    public async Task<List<Beat>> GetBeatsForDmo(Guid userId, Guid dmoId) {
        return await _context.Beats
            .AsNoTracking()
            .Where(b => b.DmoId == dmoId && b.UserId == userId)
            .OrderBy(b => b.Order)
            .ToListAsync();
    }

    public string GetContextId() {
        return _context.ContextId.ToString();
    }
}