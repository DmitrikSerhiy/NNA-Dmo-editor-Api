using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTO.Dmos;
using AutoMapper;
using Model;
using Model.Entities;

namespace API.Services
{
    public class EditorService : IEditorService
    {

        private readonly IBeatsRepository _beatsRepository;
        private readonly IMapper _mapper;
        public EditorService(
            IBeatsRepository beatsRepository, 
            IMapper mapper) {
            _beatsRepository = beatsRepository ?? throw new ArgumentNullException(nameof(beatsRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ShortDmoDto> CreateAndLoadAsync(ShortDmoDto dmoDto, Guid userId) {
            //todo: check entry params
            //todo: pass and return different dto types!
            var initialDmo = _mapper.Map<Dmo>(dmoDto);

            //todo: fix this in automapper
            initialDmo.Id = Guid.NewGuid();
            initialDmo.DateOfCreation = DateTimeOffset.UtcNow.UtcTicks;
            var created = await _beatsRepository.CreateDmoAsync(initialDmo, userId);
            
            if (!created) {
                //todo: add logger
                //todo: return some responseDto
                return null;
            }

            var dmo = await _beatsRepository.LoadShortDmoAsync(initialDmo.Id, userId);
            return dmo == null ? null : _mapper.Map<ShortDmoDto>(dmo);
        }
    }
}
