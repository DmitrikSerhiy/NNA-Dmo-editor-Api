using AutoMapper;
using Model.DTOs.Editor;
using Model.Entities;
using Model.Exceptions.Editor;
using Model.Interfaces;
using Model.Interfaces.Repositories;
using System;
using System.Threading.Tasks;

namespace API.Features.Editor.Services
{
    public class EditorService : IEditorService
    {

        private readonly IEditorRepository _editorRepository;
        private readonly IMapper _mapper;
        public EditorService(
            IEditorRepository editorRepository, 
            IMapper mapper) {
            _editorRepository = editorRepository ?? throw new ArgumentNullException(nameof(editorRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<CreatedDmoDto> CreateAndLoadAsync(CreateDmoDto dmoDto, Guid userId) {
            if (dmoDto == null) throw new ArgumentNullException(nameof(dmoDto));
            if(userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));

            //todo: validate dmo
            //todo: add unit tests

            var initialDmo = _mapper.Map<Dmo>(dmoDto);
            initialDmo.NnaUserId = userId;

            var created = await _editorRepository.CreateDmoAsync(initialDmo);
            if (!created) {
                throw new CreateDmoException();
            }


            var dmo = await _editorRepository.LoadShortDmoAsync(initialDmo.Id, userId);
            if (dmo == null) {
                throw new LoadDmoException(initialDmo.Id);
            }

            return _mapper.Map<CreatedDmoDto>(dmo);
        }
    }
}
