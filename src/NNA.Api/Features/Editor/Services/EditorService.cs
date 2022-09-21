using AutoMapper;
using NNA.Domain.DTOs.Editor;
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
        var beatToMove = _mapper.Map<Beat>(update.beatToMove);
        var beatToReplace = _mapper.Map<Beat>(update.beatToReplace);
        bool isUpdated;

        try {
            isUpdated = await _editorRepository.SwapBeatsAsync(beatToMove, beatToReplace, dmoId, userId);
        }
        catch (Exception ex) {
            throw new SwapBeatsException(ex, dmoId, userId);
        }
        
        if (!isUpdated) {
            throw new SwapBeatsException(dmoId, userId);
        }
    }
}