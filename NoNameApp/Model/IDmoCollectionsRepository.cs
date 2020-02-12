using Model.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Model {
    public interface IDmoCollectionsRepository {
        Task<List<UserDmoCollection>> GetCollectionsAsync(Guid userId);
        Task<UserDmoCollection> GetCollection(Guid collectionId, Guid userId);
        Task AddCollectionAsync(UserDmoCollection dmoCollection);
        Task<Boolean> IsCollectionExist(String collectionName, Guid userId);
        void DeleteCollection(UserDmoCollection collection);
        void UpdateCollectionName(UserDmoCollection oldDmoCollection, UserDmoCollection newDmoCollection);
        Task<UserDmoCollection> GetCollectionWithDmos(Guid userId, Guid collectionId);
        Task<Dmo> GetDmoAsync(Guid userId, Guid dmoId);
        void DeleteDmoFromCollection(Dmo dmo);
    }
}
