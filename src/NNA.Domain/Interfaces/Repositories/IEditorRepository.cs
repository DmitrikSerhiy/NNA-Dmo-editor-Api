using NNA.Domain.Entities;

namespace NNA.Domain.Interfaces.Repositories;

public interface IEditorRepository {
    Task<bool> UpdateJsonBeatsAsync(string jsonBeats, Guid id, Guid userId);
    Task<Dmo> LoadShortDmoAsync(Guid id, Guid userId);
    Task<Guid> LoadBeatIdByTempId(Guid dmoId, string tempId, Guid userId);
    Task<Dmo> LoadDmoAsync(Guid id, Guid userId);
    Task<bool> CreateDmoAsync(Dmo dmo);
    Task<bool> UpdateShortDmoAsync(Dmo dmo);
    Task<bool> InsertNewBeatAsync(Beat beat);
    Task<bool> UpdateBeatByIdAsync(Beat beat, Guid beatId);
    Task<bool> UpdateBeatByTempIdAsync(Beat beat, string? tempBeatId);
    Task<bool> DeleteBeatByIdAsync(Beat beat, Guid beatId);
    Task<bool> DeleteBeatByTempIdAsync(Beat beat);
    Task<Beat> LoadBeatForDeleteByIdAsync(Guid id, Guid dmoId);
    Task<Beat> LoadBeatForDeleteByTempIdAsync(string tempId, Guid dmoId);
    Task<bool> SetBeatOrderByTempIdAsync(Beat beat);
    Task<bool> SetBeatOrderByIdAsync(Beat beat);

    Task<bool> CreateCharacterInBeatAsync(NnaMovieCharacterInBeat nnaMovieCharacterInBeat);
    Task<bool> DeleteCharacterFromBeatByIdAsync(NnaMovieCharacterInBeat nnaMovieCharacterInBeat);
    Task<bool> DeleteCharacterFromBeatByTempIdAsync(NnaMovieCharacterInBeat nnaMovieCharacterInBeat);

    Task<bool> SanitizeTempIdsForBeatsAsync(Guid dmoId, Guid userId);
}