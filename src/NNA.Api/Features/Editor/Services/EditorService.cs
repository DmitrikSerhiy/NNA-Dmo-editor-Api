using AutoMapper;
using NNA.Domain.DTOs.CharactersInBeats;
using NNA.Domain.DTOs.Editor;
using NNA.Domain.DTOs.TagsInBeats;
using NNA.Domain.Entities;
using NNA.Domain.Exceptions.Editor;
using NNA.Domain.Interfaces;
using NNA.Domain.Interfaces.Repositories;

namespace NNA.Api.Features.Editor.Services;

public class EditorService : IEditorService {
    private readonly IEditorRepository _editorRepository;
    private readonly IMapper _mapper;

    public EditorService(
        IEditorRepository editorRepository,
        IMapper mapper) {
        _editorRepository = editorRepository ?? throw new ArgumentNullException(nameof(editorRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    [Obsolete]
    public async Task<CreatedDmoDto> CreateAndLoadDmo(CreateDmoDto? dmoDto, Guid userId) {
        if (dmoDto == null) throw new ArgumentNullException(nameof(dmoDto));
        if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));

        var initialDmo = _mapper.Map<Dmo>(dmoDto);
        initialDmo.NnaUserId = userId;
        bool isCreated;

        try {
            isCreated = await _editorRepository.CreateDmoAsync(initialDmo);
        }
        catch (Exception ex) {
            throw new CreateDmoException(ex, initialDmo.Id, userId);
        }

        if (!isCreated) {
            throw new CreateDmoException(initialDmo.Id, userId);
        }

        Dmo dmo;
        try {
            dmo = await _editorRepository.LoadShortDmoAsync(initialDmo.Id, userId);
        }
        catch (Exception ex) {
            throw new LoadShortDmoException(ex, initialDmo.Id, userId);
        }

        if (dmo == null) {
            throw new LoadShortDmoException(initialDmo.Id, userId);
        }

        return _mapper.Map<CreatedDmoDto>(dmo);
    }

    public async Task UpdateShortDmo(UpdateShortDmoDto? dmoDto, Guid userId) {
        if (dmoDto == null) throw new ArgumentNullException(nameof(dmoDto));
        if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));

        var initialDmo = _mapper.Map<Dmo>(dmoDto);
        initialDmo.NnaUserId = userId;

        bool isUpdated;
        try {
            isUpdated = await _editorRepository.UpdateShortDmoAsync(initialDmo);
        }
        catch (Exception ex) {
            throw new UpdateShortDmoException(ex, initialDmo.Id, userId);
        }

        if (!isUpdated) {
            throw new UpdateShortDmoException(initialDmo.Id, userId);
        }
    }

    public async Task<LoadedShortDmoDto> LoadShortDmo(LoadShortDmoDto? dmoDto, Guid userId) {
        if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));
        if (dmoDto == null) throw new ArgumentNullException(nameof(dmoDto));

        var initialDmo = _mapper.Map<Dmo>(dmoDto);
        initialDmo.NnaUserId = userId;

        Dmo loadedDmo;
        try {
            loadedDmo = await _editorRepository.LoadShortDmoAsync(initialDmo.Id, initialDmo.NnaUserId);
        }
        catch (Exception ex) {
            throw new LoadShortDmoException(ex, initialDmo.Id, userId);
        }

        if (loadedDmo == null) {
            throw new LoadShortDmoException(initialDmo.Id, userId);
        }

        return _mapper.Map<LoadedShortDmoDto>(loadedDmo);
    }

    public async Task UpdateDmoBeatsAsJson(UpdateDmoBeatsAsJsonDto? dmoDto, Guid userId) {
        if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));
        if (dmoDto == null) throw new ArgumentNullException(nameof(dmoDto));

        if (!Guid.TryParse(dmoDto.DmoId, out var dmoId)) {
            throw new UpdateDmoBeatsAsJsonException($"Failed to parse dmoId to GUID: {dmoDto.DmoId}");
        }

        bool isUpdated;
        try {
            isUpdated = await _editorRepository.UpdateJsonBeatsAsync(dmoDto.Data, dmoId, userId);
        }
        catch (Exception ex) {
            throw new UpdateDmoBeatsAsJsonException(ex, dmoId, userId);
        }

        if (!isUpdated) {
            throw new UpdateDmoBeatsAsJsonException(dmoId, userId);
        }
    }

    public async Task CreateBeat(CreateBeatDto? beatDto, Guid userId) {
        if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));
        if (beatDto == null) throw new ArgumentNullException(nameof(beatDto));

        var newBeat = _mapper.Map<Beat>(beatDto);
        newBeat.UserId = userId;

        bool isCreated;

        try {
            isCreated = await _editorRepository.InsertNewBeatAsync(newBeat);
        }
        catch (Exception ex) {
            throw new InsertNewBeatException(ex, newBeat.DmoId, userId);
        }

        if (!isCreated) {
            throw new InsertNewBeatException(newBeat.DmoId, userId);
        }
    }

    public async Task RemoveBeat(RemoveBeatDto? beatDto, Guid userId) {
        if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));
        if (beatDto == null) throw new ArgumentNullException(nameof(beatDto));

        if (!Guid.TryParse(beatDto.DmoId, out var dmoId)) {
            throw new DeleteBeatException($"Failed to parse dmoId to GUID: {beatDto.DmoId}");
        }

        var isBeatIdGuid = Guid.TryParse(beatDto.Id, out var beatId);
        var beat = isBeatIdGuid
            ? _mapper.Map<Beat>(beatDto)
            : new Beat { TempId = beatDto.Id, DmoId = dmoId, Order = beatDto.Order };

        bool isDeleted;
        try {
            isDeleted = isBeatIdGuid
                ? await _editorRepository.DeleteBeatByIdAsync(beat, beatId)
                : await _editorRepository.DeleteBeatByTempIdAsync(beat);
        }
        catch (Exception ex) {
            throw new DeleteBeatException(ex, dmoId, userId);
        }

        if (!isDeleted) {
            throw new DeleteBeatException(dmoId, userId);
        }
    }

    public async Task UpdateBeat(UpdateBeatDto? update, Guid userId) {
        if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));
        if (update == null) throw new ArgumentNullException(nameof(update));

        var beatToUpdate = _mapper.Map<Beat>(update);
        var isBeatIdGuid = Guid.TryParse(update.BeatId, out var beatId);
        beatToUpdate.UserId = userId;
        bool isUpdated;

        try {
            isUpdated = isBeatIdGuid
                ? await _editorRepository.UpdateBeatByIdAsync(beatToUpdate, beatId)
                : await _editorRepository.UpdateBeatByTempIdAsync(beatToUpdate, update.BeatId);
        }
        catch (Exception ex) {
            throw new UpdateBeatException(ex, update.BeatId, userId);
        }

        if (!isUpdated) {
            throw new UpdateBeatException(update.BeatId, userId);
        }
    }

    public async Task SwapBeats(SwapBeatsDto update, Guid userId) {
        if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));
        if (update == null) throw new ArgumentNullException(nameof(update));
        
        var dmoId = Guid.Parse(update.dmoId);
        var beatToMove = new Beat {
            UserId = userId,
            DmoId = dmoId,
            Order = update.beatToReplace.order
        };

        var beatToReplace = new Beat {
            UserId = userId,
            DmoId = dmoId,
            Order = update.beatToMove.order
        };
        
        var isBeatToMoveIdIsGuid = Guid.TryParse(update.beatToMove.id, out var beatIdToMove);
        var isBeatToReplaceIdIsGuid = Guid.TryParse(update.beatToReplace.id, out var beatIdToReplace);

        if (!isBeatToMoveIdIsGuid) {
            beatToMove.TempId = update.beatToMove.id;
        } else {
            beatToMove.SetIdExplicitly(beatIdToMove);
        }

        if (!isBeatToReplaceIdIsGuid) {
            beatToReplace.TempId = update.beatToReplace.id;
        } else {
            beatToReplace.SetIdExplicitly(beatIdToReplace);
        }
        
        bool isBeatToMoveUpdated;
        bool isBeatToReplaceUpdated;

        try {
            isBeatToMoveUpdated = isBeatToMoveIdIsGuid
                ? await _editorRepository.SetBeatOrderByIdAsync(beatToMove)
                : await _editorRepository.SetBeatOrderByTempIdAsync(beatToMove);
            
            isBeatToReplaceUpdated = isBeatToReplaceIdIsGuid
                ? await _editorRepository.SetBeatOrderByIdAsync(beatToReplace)
                : await _editorRepository.SetBeatOrderByTempIdAsync(beatToReplace);
        }
        catch (Exception ex) {
            throw new SwapBeatsException(ex, dmoId, userId);
        }
        
        if (!isBeatToMoveUpdated || !isBeatToReplaceUpdated) {
            throw new SwapBeatsException(dmoId, userId);
        }
    }

    public async Task MoveBeat(MoveBeatDto update, Guid userId) {
        if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));
        if (update == null) throw new ArgumentNullException(nameof(update));
        if (update.order == update.previousOrder) {
            return;
        }
        
        var dmoId = Guid.Parse(update.dmoId);
        var beatToMove = new Beat {
            UserId = userId,
            DmoId = dmoId,
            Order = update.order
        };
        
        var isBeatToMoveIdIsGuid = Guid.TryParse(update.id, out var beatIdToMove);
        if (!isBeatToMoveIdIsGuid) {
            beatToMove.TempId = update.id;
        } else {
            beatToMove.SetIdExplicitly(beatIdToMove);
        }
        
        bool isBeatToMoveUpdated;
        
        try {
            isBeatToMoveUpdated = isBeatToMoveIdIsGuid
                ? await _editorRepository.ResetBeatsOrderByIdAsync(beatToMove, update.previousOrder)
                : await _editorRepository.ResetBeatsOrderByTempIdAsync(beatToMove, update.previousOrder);
        }
        catch (Exception ex) {
            throw new MoveBeatException(ex, dmoId, userId, update.id);
        }
        
        if (!isBeatToMoveUpdated) {
            throw new MoveBeatException(dmoId, userId, update.id);
        }
    }

    public async Task AttachCharacterToBeat(AttachCharacterToBeatDto characterToBeatDto, Guid userId) {
        if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));
        if (characterToBeatDto == null) throw new ArgumentNullException(nameof(characterToBeatDto));
        
        var dmoId = Guid.Parse(characterToBeatDto.DmoId);
        var characterId = Guid.Parse(characterToBeatDto.CharacterId); 
        var isBeatIdIsGuid = Guid.TryParse(characterToBeatDto.BeatId, out var beatIdGuid);
        Guid beatId;
        bool isAttached;

        if (isBeatIdIsGuid) {
            beatId = beatIdGuid;
        } else {
            var loadedBeatId = await _editorRepository.LoadBeatIdByTempId(dmoId, characterToBeatDto.BeatId, userId);
            beatId = loadedBeatId;
        }

        var characterInBeatEntity = new NnaMovieCharacterInBeat {
            CharacterId = characterId,
            BeatId = beatId,
            TempId = characterToBeatDto.Id
        };

        try {
            isAttached = await _editorRepository.CreateCharacterInBeatAsync(characterInBeatEntity);
        } catch (Exception ex) {
            throw new AttachCharacterToBeatException(ex, dmoId, userId, beatId, characterId);
        }
        
        if (!isAttached) {
            throw new AttachCharacterToBeatException(dmoId, userId);
        }
    }

    public async Task DetachCharacterFromBeat(DetachCharacterToBeatDto characterToDetachDto, Guid userId) {
        if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));
        if (characterToDetachDto == null) throw new ArgumentNullException(nameof(characterToDetachDto));

        var isCharacterInBeatIdGuid = Guid.TryParse(characterToDetachDto.Id, out var characterInBeatIdGuid);
        var isBeatIdIsGuid = Guid.TryParse(characterToDetachDto.BeatId, out var beatIdGuid);
        var dmoId = Guid.Parse(characterToDetachDto.DmoId);
        Guid beatId;
        bool isRemoved;

        if (isBeatIdIsGuid) {
            beatId = beatIdGuid;
        } else {
            var loadedBeatId = await _editorRepository.LoadBeatIdByTempId(dmoId, characterToDetachDto.BeatId, userId);
            beatId = loadedBeatId;
        }
        
        var characterInBeatEntity = new NnaMovieCharacterInBeat {
            BeatId = beatId
        };

        if (isCharacterInBeatIdGuid) {
            characterInBeatEntity.SetIdExplicitly(characterInBeatIdGuid);
        } else {
            characterInBeatEntity.TempId = characterToDetachDto.Id;
        }
        
        try {
            isRemoved = isCharacterInBeatIdGuid
                ? await _editorRepository.DeleteCharacterFromBeatByIdAsync(characterInBeatEntity)
                : await _editorRepository.DeleteCharacterFromBeatByTempIdAsync(characterInBeatEntity);
            
        } catch (Exception ex) {
            throw new RemoveCharacterFromBeatException(ex, dmoId, userId, beatId);
        }
        
        if (!isRemoved) {
            throw new RemoveCharacterFromBeatException(dmoId, userId);
        }
    }

    public async Task AttachTagToBeat(AttachTagToBeatDto attachTagToBeatDto, Guid userId) {
        if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));
        if (attachTagToBeatDto == null) throw new ArgumentNullException(nameof(attachTagToBeatDto));
        
        var dmoId = Guid.Parse(attachTagToBeatDto.DmoId);
        var tagId = Guid.Parse(attachTagToBeatDto.TagId); 
        var isBeatIdIsGuid = Guid.TryParse(attachTagToBeatDto.BeatId, out var beatIdGuid);
        Guid beatId;
        bool isAttached;

        if (isBeatIdIsGuid) {
            beatId = beatIdGuid;
        } else {
            var loadedBeatId = await _editorRepository.LoadBeatIdByTempId(dmoId, attachTagToBeatDto.BeatId, userId);
            beatId = loadedBeatId;
        }

        var characterInBeatEntity = new NnaTagInBeat {
            TagId = tagId,
            BeatId = beatId,
            TempId = attachTagToBeatDto.Id
        };

        try {
            isAttached = await _editorRepository.CreateTagInBeatAsync(characterInBeatEntity);
        } catch (Exception ex) {
            throw new AttachTagToBeatException(ex, dmoId, userId, beatId, tagId);
        }
        
        if (!isAttached) {
            throw new AttachTagToBeatException(dmoId, userId);
        }    
    }

    public async Task DetachTagFromBeat(DetachTagFromBeatDto detachTagFromBeatDto, Guid userId)
    {
        if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));
        if (detachTagFromBeatDto == null) throw new ArgumentNullException(nameof(detachTagFromBeatDto));
        
        var isTagInBeatIdGuid = Guid.TryParse(detachTagFromBeatDto.Id, out var tagInBeatIdGuid);
        var isBeatIdIsGuid = Guid.TryParse(detachTagFromBeatDto.BeatId, out var beatIdGuid);
        var dmoId = Guid.Parse(detachTagFromBeatDto.DmoId);
        Guid beatId;
        bool isDetached;
        
        if (isBeatIdIsGuid) {
            beatId = beatIdGuid;
        } else {
            var loadedBeatId = await _editorRepository.LoadBeatIdByTempId(dmoId, detachTagFromBeatDto.BeatId, userId);
            beatId = loadedBeatId;
        }
        
        var tagInBeatEntity = new NnaTagInBeat {
            BeatId = beatId
        };

        if (isTagInBeatIdGuid) {
            tagInBeatEntity.SetIdExplicitly(tagInBeatIdGuid);
        } else {
            tagInBeatEntity.TempId = detachTagFromBeatDto.Id;
        }
        
        try {
            isDetached = isTagInBeatIdGuid
                ? await _editorRepository.DeleteTagFromBeatByIdAsync(tagInBeatEntity)
                : await _editorRepository.DeleteTagFromBeatByTempIdAsync(tagInBeatEntity);
            
        } catch (Exception ex) {
            throw new DetachTagFromBeatException(ex, dmoId, userId, beatId);
        }
        
        if (!isDetached) {
            throw new DetachTagFromBeatException(dmoId, userId);
        }
    }


    public async Task SanitizeTempIds(SanitizeTempIdsDto update, Guid userId) {
        if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));
        if (update == null) throw new ArgumentNullException(nameof(update));
        
        var dmoId = Guid.Parse(update.dmoId);

        bool isUpdated;

        try {
            isUpdated = await _editorRepository.SanitizeTempIdsForBeatsAsync(dmoId, userId);
        }
        catch (Exception ex) {
            throw new SanitizeTempIdsException(ex, dmoId, userId);
        }

        if (!isUpdated) {
            throw new SanitizeTempIdsException(dmoId, userId);
        }
    }
}