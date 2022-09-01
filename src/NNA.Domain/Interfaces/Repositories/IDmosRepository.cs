using NNA.Domain.Entities;

namespace NNA.Domain.Interfaces.Repositories;

public interface IDmosRepository : IRepository {
    Task<List<Dmo>> GetAll(Guid userId);
    Task<Dmo?> GetShortDmo(Guid userId, Guid? dmoId);
    Task<Dmo?> GetDmo(Guid userId, Guid? dmoId);
    void DeleteDmo(Dmo? dmo);
    Task<List<Beat>> GetBeatsForDmo(Guid userId, Guid dmoId);
}