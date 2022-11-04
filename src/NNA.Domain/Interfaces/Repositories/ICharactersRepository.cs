using NNA.Domain.Entities;

namespace NNA.Domain.Interfaces.Repositories;

public interface ICharactersRepository {
    Task<List<NnaMovieCharacter>> GetDmoCharactersWithBeatsAsync(Guid dmoId, CancellationToken cancellationToken);
    Task<NnaMovieCharacter?> GetCharacterByIdAsync(Guid characterId, CancellationToken cancellationToken);
    Task<NnaMovieCharacter?> GetCharacterByNameAsync(string characterName, CancellationToken cancellationToken);
    void CreateCharacter(NnaMovieCharacter? character);
    void UpdateCharactersNameAndAliases(NnaMovieCharacter? characterToUpdate, string newName, string? aliases);
    void DeleteCharacter(NnaMovieCharacter characterToDelete);
    Task<bool> IsExistAsync(string characterName, Guid dmoId, CancellationToken cancellationToken);
    Task<List<Guid>> LoadCharacterInBeatIdsAsync(Guid characterId);
}
