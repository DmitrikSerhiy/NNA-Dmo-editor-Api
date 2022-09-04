using NNA.Domain.Entities;

namespace NNA.Domain.Interfaces.Repositories;

public interface IDmosRepository : IRepository {
    Task<List<Dmo>> GetAllAsync(Guid userId, CancellationToken token);
    Task<Dmo?> GetShortDmoAsync(Guid userId, Guid? dmoId, CancellationToken token);
    Task<Dmo?> GetDmoAsync(Guid userId, Guid? dmoId, CancellationToken token);
    void DeleteDmo(Dmo? dmo);
    Task<List<Beat>> GetBeatsForDmoAsync(Guid userId, Guid dmoId, CancellationToken token);
}