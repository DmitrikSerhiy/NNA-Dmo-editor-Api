using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Model.Entities;

namespace Model
{
    public interface IDmoRepository {
        Task<ImmutableList<Dmo>> GetAll(Guid userId);
        Task Add(Dmo dmo);
    }
}
