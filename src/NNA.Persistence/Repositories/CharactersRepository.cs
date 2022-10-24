using Microsoft.EntityFrameworkCore;
using NNA.Domain.Entities;
using NNA.Domain.Interfaces;
using NNA.Domain.Interfaces.Repositories;

namespace NNA.Persistence.Repositories;

public sealed class CharactersRepository : ICharactersRepository {

    private readonly NnaContext _context;

    public CharactersRepository(IContextOrchestrator contextOrchestrator) {
        if (contextOrchestrator == null) throw new ArgumentNullException(nameof(contextOrchestrator));
        _context = (contextOrchestrator.Context as NnaContext)!;
    }
    
    public async Task<List<NnaMovieCharacter>> GetDmoCharactersAsync(Guid dmoId, CancellationToken cancellationToken) {
        if (dmoId == Guid.Empty) throw new ArgumentException("Empty dmoId", nameof(dmoId));
        return await _context.Characters
            .Where(cha => cha.DmoId == dmoId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsExistAsync(string characterName, Guid dmoId, CancellationToken cancellationToken) {
        if (characterName is null) throw new ArgumentNullException(nameof(characterName));
        if (dmoId == Guid.Empty) throw new ArgumentException("Empty dmoId", nameof(dmoId));
        return await _context.Characters
            .AnyAsync(cha => cha.Name == characterName && cha.DmoId == dmoId, cancellationToken);
    }

    public async Task<NnaMovieCharacter?> GetCharacterByIdAsync(Guid characterId, CancellationToken cancellationToken) {
        if (characterId == Guid.Empty) throw new ArgumentException("Empty characterId", nameof(characterId));
        return await _context.Characters
            .FirstOrDefaultAsync(cha => cha.Id == characterId, cancellationToken);
    }
    
    public async Task<NnaMovieCharacter?> GetCharacterByNameAsync(string characterName, CancellationToken cancellationToken) {
        if (characterName is null) throw new ArgumentNullException(nameof(characterName));
        return await _context.Characters
            .FirstOrDefaultAsync(cha => cha.Name == characterName, cancellationToken);
    }

    public void CreateCharacter(NnaMovieCharacter? character) {
        if (character is null) throw new ArgumentNullException(nameof(character));
        _context.Characters.Add(character);
    }

    public void UpdateCharactersNameAndAliases(NnaMovieCharacter? characterToUpdate, string newName, string? aliases) {
        if (characterToUpdate is null) throw new ArgumentNullException(nameof(characterToUpdate));
        if (newName is null) throw new ArgumentNullException(nameof(newName));
        characterToUpdate.Aliases = aliases;
        characterToUpdate.Name = newName;
        _context.Characters.Update(characterToUpdate);
    }

    public void DeleteCharacter(NnaMovieCharacter characterToDelete) {
        if (characterToDelete is null) throw new ArgumentNullException(nameof(characterToDelete));
        _context.Characters.Remove(characterToDelete);
    }
}
