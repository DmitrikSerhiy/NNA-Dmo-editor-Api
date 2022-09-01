using NNA.Domain.Entities;

namespace NNA.Domain.Interfaces.Repositories;

public interface IEditorRepository {
    Task<bool> UpdateJsonBeatsAsync(string jsonBeats, Guid id, Guid userId);
    Task<Dmo> LoadShortDmoAsync(Guid id, Guid userId);
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
}