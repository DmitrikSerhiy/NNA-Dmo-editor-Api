using NNA.Domain.Entities;

namespace NNA.Domain.Interfaces.Repositories;

public interface ICharactersRepository {
    Task<List<NnaMovieCharacter>> GetDmoCharactersAsync(Guid dmoId, CancellationToken cancellationToken);
    Task<NnaMovieCharacter?> GetCharacterById(Guid characterId, CancellationToken cancellationToken);
    void CreateCharacter(NnaMovieCharacter? character);
    void UpdateCharactersNameAndAliases(NnaMovieCharacter? characterToUpdate, string newName, string? aliases);
    void DeleteCharacter(NnaMovieCharacter characterToDelete);
    Task<bool> IsExist(string characterName, Guid dmoId, CancellationToken cancellationToken);
}
