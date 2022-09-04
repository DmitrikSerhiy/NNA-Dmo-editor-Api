using NNA.Domain.Entities;

namespace NNA.Domain.Interfaces.Repositories;

public interface IDmoCollectionsRepository : IRepository {
    Task<List<DmoCollection>> GetCollectionsAsync(Guid userId, CancellationToken token);
    Task<DmoCollection?> GetCollectionAsync(Guid userId, Guid? collectionId, CancellationToken token);
    void AddCollection(DmoCollection? dmoCollection);
    Task<bool> IsCollectionExistAsync(Guid userId, string collectionName, CancellationToken token);
    void DeleteCollection(DmoCollection? collection);
    void UpdateCollectionName(DmoCollection? oldDmoCollection, DmoCollection newDmoCollection);
    void AddDmoToCollection(DmoCollection dmoCollection, List<Dmo> dmos);
    bool ContainsDmo(DmoCollection dmoCollection, Guid? dmoId);
    Task<DmoCollection?> GetCollectionWithDmosAsync(Guid userId, Guid? collectionId, CancellationToken token);
    Task<Dmo?> GetDmoAsync(Guid userId, Guid? dmoId, CancellationToken token);
    Task<List<Dmo>> GetExcludedDmosAsync(Guid userId, Guid? collectionId, CancellationToken token);
    void RemoveDmoFromCollection(Dmo dmo);
}