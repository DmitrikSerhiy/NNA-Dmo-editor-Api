using Model.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Model {
    public interface IDmoCollectionsRepository {
        Task<List<UserDmoCollection>> GetCollectionsAsync(Guid userId);
        Task<UserDmoCollection> GetCollectionAsync(Guid userId, Guid collectionId);
        Task AddCollectionAsync(UserDmoCollection dmoCollection);
        Task<Boolean> IsCollectionExist(Guid userId, String collectionName);
        void DeleteCollection(UserDmoCollection collection);
        void UpdateCollectionName(UserDmoCollection oldDmoCollection, UserDmoCollection newDmoCollection);
        void AddDmoToCollection(UserDmoCollection dmoCollection, Dmo dmo);
        Boolean ContainsDmo(UserDmoCollection dmoCollection, Guid dmoId);
        Task<UserDmoCollection> GetCollectionWithDmos(Guid userId, Guid collectionId);
        Task<Dmo> GetDmoAsync(Guid userId, Guid dmoId);
        void RemoveDmoFromCollection(UserDmoCollection dmoCollection, Dmo dmo);
    }
}
