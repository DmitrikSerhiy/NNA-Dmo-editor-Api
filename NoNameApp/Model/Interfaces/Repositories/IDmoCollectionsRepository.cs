using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Model.Entities;

namespace Model.Interfaces.Repositories {
    public interface IDmoCollectionsRepository: IRepository {
        Task<List<DmoCollection>> GetCollectionsAsync(Guid userId);
        Task<DmoCollection> GetCollectionAsync(Guid userId, Guid? collectionId);
        Task AddCollectionAsync(DmoCollection dmoCollection);
        Task<bool> IsCollectionExist(Guid userId, string collectionName);
        void DeleteCollection(DmoCollection collection);
        void UpdateCollectionName(DmoCollection oldDmoCollection, DmoCollection newDmoCollection);
        void AddDmoToCollection(DmoCollection dmoCollection, List<Dmo> dmos);
        bool ContainsDmo(DmoCollection dmoCollection, Guid? dmoId);
        Task<DmoCollection> GetCollectionWithDmos(Guid userId, Guid? collectionId);
        Task<Dmo> GetDmoAsync(Guid userId, Guid? dmoId);
        Task<List<Dmo>> GetExcludedDmos(Guid userId, Guid? collectionId);
        void RemoveDmoFromCollection(DmoCollection dmoCollection, Dmo dmo);
    }
}
