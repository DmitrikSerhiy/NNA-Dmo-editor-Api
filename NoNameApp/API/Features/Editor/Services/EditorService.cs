using System;
using System.Threading.Tasks;
using AutoMapper;
using Model;
using Model.DTOs.Dmos;
using Model.Entities;
using Model.Interfaces;
using Model.Interfaces.Repositories;

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

        public async Task<ShortDmoDto> CreateAndLoadAsync(ShortDmoDto dmoDto, Guid userId) {
            //todo: check entry params
            //todo: pass and return different dto types!
            var initialDmo = _mapper.Map<Dmo>(dmoDto);

            //todo: fix this in automapper
            initialDmo.Id = Guid.NewGuid();
            initialDmo.DateOfCreation = DateTimeOffset.UtcNow.UtcTicks;
            var created = await _editorRepository.CreateDmoAsync(initialDmo, userId);
            
            if (!created) {
                //todo: add logger
                //todo: return some responseDto
                return null;
            }

            var dmo = await _editorRepository.LoadShortDmoAsync(initialDmo.Id, userId);
            return dmo == null ? null : _mapper.Map<ShortDmoDto>(dmo);
        }
    }
}
