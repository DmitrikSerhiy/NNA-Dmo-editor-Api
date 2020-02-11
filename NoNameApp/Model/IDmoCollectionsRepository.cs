using Model.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Model {
    public interface IDmoCollectionsRepository {
        Task<List<UserDmoCollection>> GetAllAsync(Guid userId);
        Task<UserDmoCollection> Get(Guid collectionId, Guid userId);
        Task AddAsync(UserDmoCollection dmoCollection);
        Task<Boolean> IsExist(String collectionName, Guid userId);
        void Delete(UserDmoCollection collection);
        void Update(UserDmoCollection oldDmoCollection, UserDmoCollection newDmoCollection);
    }
}
