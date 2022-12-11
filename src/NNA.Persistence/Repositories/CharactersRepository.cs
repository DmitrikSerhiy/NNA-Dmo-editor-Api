using Microsoft.EntityFrameworkCore;
using NNA.Domain.Entities;
using NNA.Domain.Interfaces;
using NNA.Domain.Interfaces.Repositories;

namespace NNA.Persistence.Repositories;

internal sealed class CharactersRepository : CommonRepository, ICharactersRepository {
    public CharactersRepository(IContextOrchestrator contextOrchestrator): base(contextOrchestrator) { }
    
    public async Task<List<NnaMovieCharacter>> GetDmoCharactersWithBeatsAsync(Guid dmoId, CancellationToken cancellationToken) {
        if (dmoId == Guid.Empty) throw new ArgumentException("Empty dmoId", nameof(dmoId));
        return await Context.Characters
            .Where(cha => cha.DmoId == dmoId)
            .Include(cha => cha.Beats)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsExistAsync(string characterName, Guid dmoId, CancellationToken cancellationToken) {
        if (characterName is null) throw new ArgumentNullException(nameof(characterName));
        if (dmoId == Guid.Empty) throw new ArgumentException("Empty dmoId", nameof(dmoId));
        return await Context.Characters
            .AnyAsync(cha => cha.Name == characterName && cha.DmoId == dmoId, cancellationToken);
    }

    public async Task<List<Guid>> LoadCharacterInBeatIdsAsync(Guid characterId) {
        return await Context.CharacterInBeats
            .AsTracking()
            .Where(cha => cha.CharacterId == characterId)
            .Select(cha => cha.Id)
            .ToListAsync();
    }

    public async Task<NnaMovieCharacter?> GetCharacterByIdAsync(Guid characterId, CancellationToken cancellationToken) {
        if (characterId == Guid.Empty) throw new ArgumentException("Empty characterId", nameof(characterId));
        return await Context.Characters
            .FirstOrDefaultAsync(cha => cha.Id == characterId, cancellationToken);
    }
    
    public async Task<NnaMovieCharacter?> GetCharacterByNameAsync(string characterName, CancellationToken cancellationToken) {
        if (characterName is null) throw new ArgumentNullException(nameof(characterName));
        return await Context.Characters
            .FirstOrDefaultAsync(cha => cha.Name == characterName, cancellationToken);
    }

    public void CreateCharacter(NnaMovieCharacter? character) {
        if (character is null) throw new ArgumentNullException(nameof(character));
        Context.Characters.Add(character);
    }

    public void UpdateCharacter(NnaMovieCharacter? update) {
        if (update is null) throw new ArgumentNullException(nameof(update));
        Context.Characters.Update(update);
    }

    public void DeleteCharacter(NnaMovieCharacter characterToDelete) {
        if (characterToDelete is null) throw new ArgumentNullException(nameof(characterToDelete));
        Context.Characters.Remove(characterToDelete);
    }

    public void AddCharacterConflict(NnaMovieCharacterConflictInDmo conflict) {
        if (conflict is null) throw new ArgumentNullException(nameof(conflict));
        Context.NnaMovieCharacterConflicts.Add(conflict);
    }
}
