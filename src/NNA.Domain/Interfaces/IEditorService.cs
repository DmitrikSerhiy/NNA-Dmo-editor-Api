using NNA.Domain.DTOs.CharactersInBeats;
using NNA.Domain.DTOs.Editor;

namespace NNA.Domain.Interfaces;

public interface IEditorService {
    Task<CreatedDmoDto> CreateAndLoadDmo(CreateDmoDto dto, Guid userId);
    Task UpdateShortDmo(UpdateShortDmoDto dmoDto, Guid userId);
    Task<LoadedShortDmoDto> LoadShortDmo(LoadShortDmoDto dmoDto, Guid userId);
    Task UpdateDmoBeatsAsJson(UpdateDmoBeatsAsJsonDto dmoDto, Guid userId);
    Task CreateBeat(CreateBeatDto beatDto, Guid userId);
    Task RemoveBeat(RemoveBeatDto beatDto, Guid userId);
    Task UpdateBeat(UpdateBeatDto update, Guid userId);
    Task SwapBeats(SwapBeatsDto update, Guid userId);
    Task AttachCharacterToBeat(AttachCharacterToBeatDto characterToBeatDto, Guid userId);
    Task DetachCharacterFromBeat(DetachCharacterToBeatDto characterToDetachDto, Guid userId);
    
    Task SanitizeTempIds(SanitizeTempIdsDto update, Guid userId);
}