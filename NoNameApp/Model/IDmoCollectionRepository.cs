using Model.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Model {
    public interface IDmoCollectionRepository {
        Task<List<UserDmoCollection>> GetAllAsync(Guid userId);
        Task<UserDmoCollection> Get(Guid collectionId, Guid userId);
        Task AddAsync(UserDmoCollection dmoCollection);
        Task<Boolean> IsExist(Guid collectionId, Guid userId);
        void Delete(UserDmoCollection collection);
        void Update(UserDmoCollection oldDmoCollection, UserDmoCollection newDmoCollection);
    }
}
