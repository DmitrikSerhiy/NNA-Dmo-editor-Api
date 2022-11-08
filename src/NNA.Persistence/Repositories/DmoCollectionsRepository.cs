using Microsoft.EntityFrameworkCore;
using NNA.Domain.Entities;
using NNA.Domain.Interfaces;
using NNA.Domain.Interfaces.Repositories;

namespace NNA.Persistence.Repositories;

internal sealed class DmoCollectionsRepository : CommonRepository, IDmoCollectionsRepository {
    public DmoCollectionsRepository(IContextOrchestrator contextOrchestrator): base(contextOrchestrator) { }

    public async Task<List<DmoCollection>> GetCollectionsAsync(Guid userId, CancellationToken token) {
        return await Context.DmoCollections
            .Where(dmo => dmo.NnaUserId == userId)
            .Include(dmo => dmo.DmoCollectionDmos)
            .AsNoTracking()
            .OrderByDescending(dmo => dmo.DateOfCreation)
            .ToListAsync(token);
    }

    public async Task<DmoCollection?> GetCollectionAsync(Guid userId, Guid? collectionId, CancellationToken token) {
        if (!collectionId.HasValue) throw new ArgumentNullException(nameof(collectionId));
        return await Context.DmoCollections
            .FirstOrDefaultAsync(udc => udc.Id == collectionId && udc.NnaUserId == userId, token);
    }

    public async Task<Dmo?> GetDmoAsync(Guid userId, Guid? dmoId, CancellationToken token) {
        if (!dmoId.HasValue) throw new ArgumentNullException(nameof(dmoId));
        return await Context.Dmos.FirstOrDefaultAsync(d => d.Id == dmoId && d.NnaUserId == userId, token);
    }

    public async Task<bool> IsCollectionExistAsync(Guid userId, string collectionName, CancellationToken token) {
        if (string.IsNullOrWhiteSpace(collectionName)) throw new ArgumentNullException(nameof(collectionName));
        return await Context.DmoCollections.AnyAsync(udc =>
            udc.CollectionName.Equals(collectionName) && udc.NnaUserId == userId, token);
    }

    public void UpdateCollectionName(DmoCollection? oldDmoCollection, DmoCollection newDmoCollection) {
        if (oldDmoCollection == null) throw new ArgumentNullException(nameof(oldDmoCollection));
        if (newDmoCollection == null) throw new ArgumentNullException(nameof(newDmoCollection));

        oldDmoCollection.CollectionName = newDmoCollection.CollectionName;
        Context.DmoCollections.Update(oldDmoCollection);
    }

    public void AddCollection(DmoCollection? dmoCollection) {
        if (dmoCollection == null) throw new ArgumentNullException(nameof(dmoCollection));
        Context.DmoCollections.Add(dmoCollection);
    }

    public void DeleteCollection(DmoCollection? collection) {
        if (collection == null) throw new ArgumentNullException(nameof(collection));
        collection.DmoCollectionDmos.Clear();
        Context.DmoCollections.Remove(collection);
    }

    public async Task<DmoCollection?> GetCollectionWithDmosAsync(Guid userId, Guid? collectionId, CancellationToken token) {
        if (!collectionId.HasValue) throw new ArgumentNullException(nameof(collectionId));
        var dmoCollection = await Context.DmoCollections
            .Where(dmo => dmo.NnaUserId == userId && dmo.Id == collectionId)
            .Include(dmoCollection => dmoCollection.DmoCollectionDmos)
            .ThenInclude(dmo => dmo.Dmo)
            .FirstOrDefaultAsync(token);

        if (dmoCollection == null) {
            return null;
        }

        dmoCollection.DmoCollectionDmos =
            dmoCollection.DmoCollectionDmos.OrderByDescending(d => d.Dmo!.DateOfCreation).ToList();
        return dmoCollection;
    }

    public void AddDmoToCollection(DmoCollection dmoCollection, List<Dmo> dmos) {
        if (dmoCollection == null) throw new ArgumentNullException(nameof(dmoCollection));
        if (dmos == null || dmos.Count == 0) throw new ArgumentNullException(nameof(dmos));
        foreach (var dmo in dmos) {
            dmoCollection.DmoCollectionDmos.Add(new DmoCollectionDmo {
                DmoId = dmo.Id,
                Dmo = dmo,
                DmoCollection = dmoCollection,
                DmoCollectionId = dmoCollection.Id
            });
        }
    }

    public bool ContainsDmo(DmoCollection dmoCollection, Guid? dmoId) {
        if (dmoCollection == null) throw new ArgumentNullException(nameof(dmoCollection));
        if (!dmoId.HasValue) throw new ArgumentNullException(nameof(dmoId));

        return dmoCollection.DmoCollectionDmos.Any(d => d.DmoId == dmoId);
    }

    public async Task<List<Dmo>> GetExcludedDmosAsync(Guid userId, Guid? collectionId, CancellationToken token) {
        if (!collectionId.HasValue) throw new ArgumentNullException(nameof(collectionId));

        return await Context.Dmos
            .Where(d => d.NnaUserId == userId &&
                        d.DmoCollectionDmos.All(dc => dc.DmoCollectionId != collectionId.Value))
            .ToListAsync(token);
    }

    public void RemoveDmoFromCollection(Dmo dmo) {
        if (dmo == null) throw new ArgumentNullException(nameof(dmo));

        var dmoDmoCollection = dmo.DmoCollectionDmos.First(d => d.DmoId == dmo.Id);
        dmoDmoCollection.DmoCollection = null;
    }
}