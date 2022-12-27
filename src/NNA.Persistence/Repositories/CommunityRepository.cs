using Microsoft.EntityFrameworkCore;
using NNA.Domain.Entities;
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

    public Task<int> GetPublishedAmountAsync(CancellationToken token) {
        return Context.Dmos
            .Where(dmo => dmo.Published)
            .CountAsync(token);
    }
}