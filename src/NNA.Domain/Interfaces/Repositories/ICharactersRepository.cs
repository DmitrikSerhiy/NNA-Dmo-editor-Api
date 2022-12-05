using NNA.Domain.Entities;

namespace NNA.Domain.Interfaces.Repositories;

public interface ICharactersRepository : IRepository {
    Task<List<NnaMovieCharacter>> GetDmoCharactersWithBeatsAsync(Guid dmoId, CancellationToken cancellationToken);
    Task<NnaMovieCharacter?> GetCharacterByIdAsync(Guid characterId, CancellationToken cancellationToken);
    Task<NnaMovieCharacter?> GetCharacterByNameAsync(string characterName, CancellationToken cancellationToken);
    void CreateCharacter(NnaMovieCharacter? character);
    void UpdateCharacter(NnaMovieCharacter? characterToUpdate);
    void DeleteCharacter(NnaMovieCharacter characterToDelete);
    Task<bool> IsExistAsync(string characterName, Guid dmoId, CancellationToken cancellationToken);
    Task<List<Guid>> LoadCharacterInBeatIdsAsync(Guid characterId);
    void AddCharacterConflict(NnaMovieCharacterConflictInDmo conflict);
    Task<NnaMovieCharacterConflictInDmo?> LoadNnaCharacterConflictAsync(Guid id, CancellationToken cancellationToken);
    void UpdateCharacterConflictInDmo(NnaMovieCharacterConflictInDmo? conflictInDmo);
}
