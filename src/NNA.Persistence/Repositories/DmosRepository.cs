﻿using Microsoft.EntityFrameworkCore;
using NNA.Domain.Entities;
using NNA.Domain.Interfaces;
using NNA.Domain.Interfaces.Repositories;

namespace NNA.Persistence.Repositories;

internal sealed class DmosRepository : CommonRepository, IDmosRepository {
    public DmosRepository(IContextOrchestrator contextOrchestrator): base(contextOrchestrator) { }
    public async Task<Dmo?> GetById(Guid id, CancellationToken token, bool withTracking = false) {
        if (id == Guid.Empty) throw new ArgumentException(nameof(id));
        return withTracking
            ? await Context.Dmos.FirstOrDefaultAsync(dmo => dmo.Id == id, token)
            : await Context.Dmos.AsNoTracking().FirstOrDefaultAsync(dmo => dmo.Id == id, token);
    }
    
    public async Task<Dmo?> GetByIdWithCharactersAndConflicts(Guid id, CancellationToken token, bool withTracking = false) {
        if (id == Guid.Empty) throw new ArgumentException(nameof(id));
        return withTracking
            ? await Context.Dmos.Include(dmo => dmo.Characters).Include(cha => cha.Conflicts).AsSplitQuery().FirstOrDefaultAsync(dmo => dmo.Id == id, token)
            : await Context.Dmos.Include(dmo => dmo.Characters).Include(cha => cha.Conflicts).AsSplitQuery().AsNoTracking().FirstOrDefaultAsync(dmo => dmo.Id == id, token);
    }
    
    public async Task<Dmo?> GetShortById(Guid id, CancellationToken token, bool withTracking = false) {
        if (id == Guid.Empty) throw new ArgumentException(nameof(id));
        var query = withTracking 
                ? Context.Dmos.Where(dmo => dmo.Id == id)
                : Context.Dmos.AsNoTracking().Where(dmo => dmo.Id == id);

        return await query.Select(dmo => new Dmo {
                DmoStatus = dmo.DmoStatus,
                MovieTitle = dmo.MovieTitle,
                Published = dmo.Published
            })
            .FirstOrDefaultAsync(token);
    }

    public async Task<List<Dmo>> GetAllAsync(Guid userId, CancellationToken token) {
        if (userId == Guid.Empty) throw new ArgumentException(nameof(userId));
        return await Context.Dmos.Where(dmo => dmo.NnaUserId == userId)
            .OrderByDescending(dmo => dmo.DateOfCreation)
            .ToListAsync(token);
    }

    public async Task<NnaMovieCharacterConflictInDmo?> GetNnaMovieCharacterConflictById(Guid conflictId, CancellationToken token) {
        if (conflictId == Guid.Empty) throw new ArgumentException(nameof(conflictId));
        return await Context.NnaMovieCharacterConflicts
            .FirstOrDefaultAsync(c => c.Id == conflictId, token);
    }

    public async Task<List<NnaMovieCharacterConflictInDmo>> GetNnaMovieCharacterConflictByPairId(Guid conflictPairId, CancellationToken token) {
        if (conflictPairId == Guid.Empty) throw new ArgumentException(nameof(conflictPairId));
        return await Context.NnaMovieCharacterConflicts
            .Where(c => c.PairId == conflictPairId)
            .ToListAsync(token);
    }

    public async Task<Dmo?> GetDmoForDelete(Guid userId, Guid dmoId, CancellationToken token) {
        return await Context.Dmos
            .Include(d => d.DmoCollectionDmos)
            .ThenInclude(dd => dd.DmoCollection)
            .Include(d => d.Beats)
            .Include(d => d.Conflicts)
            .AsSplitQuery()
            .FirstOrDefaultAsync(d => d.NnaUserId == userId && d.Id == dmoId, token);
    }
    
    public void DeleteDmo(Dmo? dmo) {
        if (dmo == null) throw new ArgumentNullException(nameof(dmo));
        dmo.DmoCollectionDmos.Clear();
        Context.Beats.RemoveRange(dmo.Beats);
        Context.Dmos.Remove(dmo);
    }

    public void UpdateDmo(Dmo? dmo) {
        if (dmo is null) throw new ArgumentNullException(nameof(dmo));
        Context.Update(dmo);
    }
    
    public void UpdateConflictInDmo(NnaMovieCharacterConflictInDmo? conflict) {
        if (conflict is null) throw new ArgumentNullException(nameof(conflict));
        Context.Update(conflict);
    }

    public void CreateConflictInDmo(NnaMovieCharacterConflictInDmo? conflict) {
        if (conflict is null) throw new ArgumentNullException(nameof(conflict));
        Context.NnaMovieCharacterConflicts.Add(conflict);
    }

    public void DeleteConflictInDmo(NnaMovieCharacterConflictInDmo? conflict) {
        if (conflict is null) throw new ArgumentNullException(nameof(conflict));
        Context.NnaMovieCharacterConflicts.Remove(conflict);    
    }

    public async Task<Dmo?> GetDmoWithDataAsync(Guid dmoId, CancellationToken cancellationToken) {
        return await Context.Dmos
            .AsNoTracking()
            .Include(d => d.Beats)
                .ThenInclude(dc => dc.Characters)
                    .ThenInclude(cha => cha.Character)
            .Include(d => d.Beats)
                .ThenInclude(tig => tig.Tags)
                    .ThenInclude(t => t.Tag)
            .Include(d => d.Characters)
            .AsSplitQuery()
            .FirstOrDefaultAsync(b => b.Id == dmoId, cancellationToken);
    }

    public async Task<List<Beat>> LoadBeatsWithCharactersAsync(Guid userId, Guid dmoId) {
        return await Context.Beats
            .AsTracking()
            .Where(d => d.DmoId == dmoId && d.UserId == userId)
            .Include(d => d.Characters)
            .OrderBy(d => d.Order)
            .ToListAsync();
    }
    
    public async Task<List<Beat>> LoadBeatsWithNestedEntitiesAsync(Guid userId, Guid dmoId) {
        return await Context.Beats
            .AsTracking()
            .Where(d => d.DmoId == dmoId && d.UserId == userId)
            .Include(d => d.Characters)
            .Include(d => d.Tags)
            .AsSplitQuery()
            .OrderBy(d => d.Order)
            .ToListAsync();
    }
    
    public async Task<List<Beat>> LoadBeatsAsync(Guid userId, Guid dmoId) {
        return await Context.Beats
            .AsTracking()
            .Where(d => d.DmoId == dmoId && d.UserId == userId)
            .ToListAsync();
    }
}