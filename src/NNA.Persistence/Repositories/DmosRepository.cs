using Microsoft.EntityFrameworkCore;
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
            ? await Context.Dmos.Include(dmo => dmo.Characters).ThenInclude(cha => cha.Conflicts).FirstOrDefaultAsync(dmo => dmo.Id == id, token)
            : await Context.Dmos.Include(dmo => dmo.Characters).ThenInclude(cha => cha.Conflicts).AsNoTracking().FirstOrDefaultAsync(dmo => dmo.Id == id, token);
    }
    
    public async Task<Dmo?> GetShortById(Guid id, CancellationToken token, bool withTracking = false) {
        if (id == Guid.Empty) throw new ArgumentException(nameof(id));
        var query = withTracking 
                ? Context.Dmos.Where(dmo => dmo.Id == id)
                : Context.Dmos.AsNoTracking().Where(dmo => dmo.Id == id);

        return await query.Select(dmo => new Dmo {
                DmoStatus = dmo.DmoStatus,
                MovieTitle = dmo.MovieTitle
            })
            .FirstOrDefaultAsync(token);
    }

    public async Task<List<Dmo>> GetAllAsync(Guid userId, CancellationToken token) {
        return await Context.Dmos.Where(dmo => dmo.NnaUserId == userId)
            .OrderByDescending(dmo => dmo.DateOfCreation)
            .ToListAsync(token);
    }

    public async Task<Dmo?> GetShortDmoAsync(Guid userId, Guid? dmoId, CancellationToken token) {
        if (!dmoId.HasValue) throw new ArgumentNullException(nameof(dmoId));
        return await Context.Dmos.FirstOrDefaultAsync(d => d.NnaUserId == userId && d.Id == dmoId.Value, token);
    }

    public async Task<Dmo?> GetDmoAsync(Guid userId, Guid? dmoId, CancellationToken token) {
        if (!dmoId.HasValue) throw new ArgumentNullException(nameof(dmoId));
        return await Context.Dmos
            .Include(d => d.DmoCollectionDmos)
            .ThenInclude(dd => dd.DmoCollection)
            .Include(d => d.Beats)
            .FirstOrDefaultAsync(d => d.NnaUserId == userId && d.Id == dmoId.Value, token);
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

    public async Task<Dmo?> GetDmoWithDataAsync(Guid userId, Guid dmoId, CancellationToken cancellationToken) {
        return await Context.Dmos
            .AsNoTracking()
            .Include(d => d.Beats)
                .ThenInclude(dc => dc.Characters)
                    .ThenInclude(cha => cha.Character)
            .Include(d => d.Characters)
            .FirstOrDefaultAsync(b => b.Id == dmoId && b.NnaUserId == userId, cancellationToken);
    }

    public async Task<List<Beat>> LoadBeatsWithCharactersAsync(Guid userId, Guid dmoId) {
        return await Context.Beats
            .AsTracking()
            .Where(d => d.DmoId == dmoId && d.UserId == userId)
            .Include(d => d.Characters)
            .ToListAsync();
    }
    
    public async Task<List<Beat>> LoadBeatsAsync(Guid userId, Guid dmoId) {
        return await Context.Beats
            .AsTracking()
            .Where(d => d.DmoId == dmoId && d.UserId == userId)
            .ToListAsync();
    }
}