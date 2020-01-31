using Model.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Model {
    public interface IDmoCollectionRepository {
        Task<List<UserDmoCollection>> GetAllAsync(Guid userId);
        Task AddAsync(UserDmoCollection dmoCollection);
        Task<Boolean> IsExist(String collectionName, Guid userId);
    }
}
