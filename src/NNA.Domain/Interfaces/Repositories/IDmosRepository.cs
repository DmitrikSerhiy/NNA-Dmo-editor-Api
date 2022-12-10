using NNA.Domain.Entities;

namespace NNA.Domain.Interfaces.Repositories;

public interface IDmosRepository : IRepository {
    Task<Dmo?> GetById(Guid id, CancellationToken token, bool withTracking = false);
    Task<Dmo?> GetByIdWithCharactersAndConflicts(Guid id, CancellationToken token, bool withTracking = false);
    Task<Dmo?> GetShortById(Guid id, CancellationToken token, bool withTracking = false);
    Task<List<Dmo>> GetAllAsync(Guid userId, CancellationToken token);
    Task<NnaMovieCharacterConflictInDmo?> GetNnaMovieCharacterConflictById(Guid conflictId, CancellationToken token);
    Task<List<NnaMovieCharacterConflictInDmo>> GetNnaMovieCharacterConflictByPairId(Guid conflictPairId, CancellationToken token);

    Task<Dmo?> GetShortDmoAsync(Guid userId, Guid? dmoId, CancellationToken token);
    Task<Dmo?> GetDmoAsync(Guid userId, Guid? dmoId, CancellationToken token);
    void UpdateDmo(Dmo? dmo);
    void DeleteDmo(Dmo? dmo);
    void UpdateConflictInDmo(NnaMovieCharacterConflictInDmo? conflict);
    void CreateConflictInDmo(NnaMovieCharacterConflictInDmo? conflict);
    void DeleteConflictInDmo(NnaMovieCharacterConflictInDmo? conflict);
    
    Task<Dmo?> GetDmoWithDataAsync(Guid userId, Guid dmoId, CancellationToken cancellationToken);
    Task<List<Beat>> LoadBeatsWithCharactersAsync(Guid userId, Guid dmoId);
    Task<List<Beat>> LoadBeatsAsync(Guid userId, Guid dmoId);
}