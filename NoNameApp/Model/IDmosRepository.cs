using Model.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Model
{
    public interface IDmosRepository {
        Task<List<Dmo>> GetAll(Guid userId);
        Task<Dmo> GetShortDmo(Guid userId, Guid? dmoId);
        Task<Dmo> GetDmo(Guid userId, Guid? dmoId);
        void DeleteDmo(Dmo dmo);
    }
}
