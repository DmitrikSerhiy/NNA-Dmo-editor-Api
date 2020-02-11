using Model.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Model
{
    public interface IDmoCollectionRepository {
        Task<List<UserDmoCollection>> GetAllDmoAsync(Guid userId, Guid collectionId);
    }
}
