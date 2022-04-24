using AutoMapper;
using Model.DTOs.Editor;
using Model.Entities;
using Model.Exceptions.Editor;
using Model.Interfaces;
using Model.Interfaces.Repositories;
using System;
using System.Threading.Tasks;

namespace API.Features.Editor.Services {
    public class EditorService : IEditorService {

        private readonly IEditorRepository _editorRepository;
        private readonly IMapper _mapper;

        public EditorService(
            IEditorRepository editorRepository,
            IMapper mapper) {
            _editorRepository = editorRepository ?? throw new ArgumentNullException(nameof(editorRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<CreatedDmoDto> CreateAndLoadDmo(CreateDmoDto dmoDto, Guid userId) {
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

        public async Task UpdateShortDmo(UpdateShortDmoDto dmoDto, Guid userId) {
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

        public async Task<LoadedShortDmoDto> LoadShortDmo(LoadShortDmoDto dmoDto, Guid userId) {
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

        public async Task UpdateDmoBeatsAsJson(UpdateDmoBeatsAsJsonDto dmoDto, Guid userId) {
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

    }
}
