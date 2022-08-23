using Microsoft.EntityFrameworkCore;
using Model.Entities;
using Model.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Persistence.Repositories;
internal sealed class DmoCollectionsRepository : IDmoCollectionsRepository {
    private readonly NnaContext _context;
    public DmoCollectionsRepository(ContextOrchestrator contextOrchestrator) {
        if (contextOrchestrator == null) throw new ArgumentNullException(nameof(contextOrchestrator));

        _context = contextOrchestrator.Context;
        Console.WriteLine($"From DmoCollectionsRepository {GetContextId()}");
    }

    public async Task<List<DmoCollection>> GetCollectionsAsync(Guid userId) {
        return await _context.DmoCollections
            .Where(d => d.NnaUserId == userId)
            .Include(d => d.DmoCollectionDmos)
            .AsNoTracking()
            .OrderByDescending(d => d.DateOfCreation)
            .ToListAsync();
    }

    public async Task<DmoCollection> GetCollectionAsync(Guid userId, Guid? collectionId) {
        if(!collectionId.HasValue) throw new ArgumentNullException(nameof(collectionId));
        return await _context.DmoCollections
            .FirstOrDefaultAsync(udc => udc.Id == collectionId && udc.NnaUserId == userId);
    }

    public async Task<Dmo> GetDmoAsync(Guid userId, Guid? dmoId) {
        if (!dmoId.HasValue) throw new ArgumentNullException(nameof(dmoId));
        return await _context.Dmos.FirstOrDefaultAsync(d => d.Id == dmoId && d.NnaUserId == userId);
    }

    public async Task<bool> IsCollectionExist(Guid userId, string collectionName) {
        if(string.IsNullOrWhiteSpace(collectionName)) throw new ArgumentNullException(nameof(collectionName));
        return await _context.DmoCollections.AnyAsync(udc =>
            udc.CollectionName.Equals(collectionName) && udc.NnaUserId == userId);
    }

    public void UpdateCollectionName(DmoCollection oldDmoCollection, DmoCollection newDmoCollection) {
        if (oldDmoCollection == null) throw new ArgumentNullException(nameof(oldDmoCollection));
        if (newDmoCollection == null) throw new ArgumentNullException(nameof(newDmoCollection));

        oldDmoCollection.CollectionName = newDmoCollection.CollectionName;
        _context.DmoCollections.Update(oldDmoCollection);
    }

    public async Task AddCollectionAsync(DmoCollection dmoCollection) {
        if (dmoCollection == null) throw new ArgumentNullException(nameof(dmoCollection));
        await _context.DmoCollections.AddAsync(dmoCollection);
    }

    public void DeleteCollection(DmoCollection collection) {
        if (collection == null) throw new ArgumentNullException(nameof(collection));
        collection.DmoCollectionDmos.Clear();
        _context.DmoCollections.Remove(collection);
    }

    public async Task<DmoCollection> GetCollectionWithDmos(Guid userId, Guid? collectionId) {
        if (!collectionId.HasValue) throw new ArgumentNullException(nameof(collectionId));
        var dmoCollection = await _context.DmoCollections
            .Where(d => d.NnaUserId == userId && d.Id == collectionId)
            .Include(dc => dc.DmoCollectionDmos)
            .ThenInclude(d => d.Dmo)
            .FirstOrDefaultAsync();

        dmoCollection.DmoCollectionDmos = dmoCollection.DmoCollectionDmos.OrderByDescending(d => d.Dmo.DateOfCreation).ToList();
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

    public async Task<List<Dmo>> GetExcludedDmos(Guid userId, Guid? collectionId) {
        if (!collectionId.HasValue) throw new ArgumentNullException(nameof(collectionId));

        return await _context.Dmos
            .Where(d => d.NnaUserId == userId && d.DmoCollectionDmos.All(dc => dc.DmoCollectionId != collectionId.Value))
            .ToListAsync();
    }

    public void RemoveDmoFromCollection(DmoCollection dmoCollection, Dmo dmo) {
        if (dmoCollection == null) throw new ArgumentNullException(nameof(dmoCollection));
        if (dmo == null) throw new ArgumentNullException(nameof(dmo));

        var dmoDmoCollection = dmo.DmoCollectionDmos.First(d => d.DmoId == dmo.Id);
        dmoDmoCollection.DmoCollection = null;
    }

    public string GetContextId() {
        return _context.ContextId.ToString();
    }
}