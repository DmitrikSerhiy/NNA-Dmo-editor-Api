using NNA.Domain.Entities;

namespace NNA.Domain.Interfaces.Repositories;

public interface IDmosRepository : IRepository {
    Task<Dmo?> GetById(Guid id, CancellationToken token);
    Task<List<Dmo>> GetAllAsync(Guid userId, CancellationToken token);
    Task<Dmo?> GetShortDmoAsync(Guid userId, Guid? dmoId, CancellationToken token);
    Task<Dmo?> GetDmoAsync(Guid userId, Guid? dmoId, CancellationToken token);
    void UpdateDmoDetails(Dmo? dmo);
    void DeleteDmo(Dmo? dmo);
    Task<Dmo?> GetDmoWithDataAsync(Guid userId, Guid dmoId, CancellationToken cancellationToken);
    Task<List<Beat>> LoadBeatsWithCharactersAsync(Guid userId, Guid dmoId);
    Task<List<Beat>> LoadBeatsAsync(Guid userId, Guid dmoId);
}