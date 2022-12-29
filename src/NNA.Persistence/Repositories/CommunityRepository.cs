using Microsoft.EntityFrameworkCore;
using NNA.Domain.Entities;
using NNA.Domain.Enums;
using NNA.Domain.Interfaces;
using NNA.Domain.Interfaces.Repositories;

namespace NNA.Persistence.Repositories; 

public sealed class CommunityRepository : CommonRepository, ICommunityRepository {
    
    public CommunityRepository(IContextOrchestrator contextOrchestrator): base(contextOrchestrator) { }
    
    public Task<List<Dmo>> GetPublishedDmosAsync(int pageSize, int pageNumber, CancellationToken token) {
        return Context.Dmos
            .Where(dmo => dmo.Published)
            .Include(dmo => dmo.NnaUser)
            .AsNoTracking()
            .OrderByDescending(dmo => dmo.PublishDate)
            .Skip(pageSize * pageNumber)
            .Take(pageSize)
            .ToListAsync(token);
    }

    public async Task<int> GetPublishedDmosAmountAsync(List<Guid> dmoIdsToIgnore, string searchBy, CancellationToken token) {
        var query = Context.Dmos
            .Include(dmo => dmo.NnaUser)
            .Where(dmo => dmo.Published &&
                          (dmo.MovieTitle.Contains(searchBy) ||
                           (dmo.Name != null && dmo.Name.Contains(searchBy)) ||
                           dmo.NnaUser.UserName.Contains(searchBy)))
            .AsNoTracking();

        if (dmoIdsToIgnore.Count > 0) {
            query = query.Where(dmo => !dmoIdsToIgnore.Contains(dmo.Id));
        }
        
        return await query.CountAsync(token);
    }
    
    public async Task<List<Dmo>> GetPublishedDmosAsync(List<Guid> dmoIdsToIgnore, string searchBy, int amount, CancellationToken token) {
        var query = Context.Dmos
            .Include(dmo => dmo.NnaUser)
            .Where(dmo => dmo.Published &&
                          (dmo.MovieTitle.Contains(searchBy) ||
                           (dmo.Name != null && dmo.Name.Contains(searchBy)) ||
                           dmo.NnaUser.UserName.Contains(searchBy)))
            .AsNoTracking();
        
        if (dmoIdsToIgnore.Count > 0) {
            query = query.Where(dmo => !dmoIdsToIgnore.Contains(dmo.Id));
        }

        return await query.OrderByDescending(dmo => dmo.PublishDate)
            .Take(amount)
            .ToListAsync(token);
    }

    public Task<int> GetPublishedAmountAsync(CancellationToken token) {
        return Context.Dmos
            .Where(dmo => dmo.Published)
            .AsNoTracking()
            .CountAsync(token);
    }

    public async Task<int> GetBeatsCount(Guid dmoId, CancellationToken token) {
        return await Context.Beats
            .Where(b => b.DmoId == dmoId)
            .AsNoTracking()
            .CountAsync(token);
    }

    public async Task<int> GetNonAestheticBeatsCount(Guid dmoId, CancellationToken token) {
        return await Context.Beats
            .Where(b => b.DmoId == dmoId && b.Type != BeatType.AestheticBeat)
            .AsNoTracking()
            .CountAsync(token);
    }

    public async Task<int> GetCharactersCount(Guid dmoId, CancellationToken token) {
        return await Context.Characters
            .Where(cha => cha.DmoId == dmoId)
            .AsNoTracking()
            .CountAsync(token);
    }

    public async Task<(string?, string?)> GetDmoPremiseAndControllingIdea(Guid dmoId, CancellationToken token) {
        var dmoData = await Context.Dmos
            .Where(dmo => dmo.Id == dmoId)
            .AsNoTracking()
            .Select(dmo => new { premise = dmo.Premise, controllingIdea = dmo.ControllingIdea })
            .FirstOrDefaultAsync(token);

        return (dmoData!.premise, dmoData.controllingIdea);
    }
}