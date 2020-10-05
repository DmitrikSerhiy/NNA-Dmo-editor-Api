using API.DTO.DmoCollections;
using API.DTO.Dmos;
using API.Infrastructure.Authentication;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DmosController : ControllerBase {
        private readonly CurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly IDmosRepository _dmosRepository;

        public DmosController(
            CurrentUserService currentUserService, 
            IMapper mapper, 
            IDmosRepository dmosRepository) {
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _dmosRepository = dmosRepository ?? throw new ArgumentNullException(nameof(dmosRepository));
        }

        [HttpGet]
        [Route("")]
        public async Task<ActionResult<DmoShortDto[]>> GetDmos() {
            var user = await _currentUserService.GetAsync();
            var dmos = await _dmosRepository.GetAll(user.Id);
            return Ok(dmos.Select(_mapper.Map<DmoShortDto>).ToArray());
        }

        [HttpDelete]
        [Route("")]
        public async Task<ActionResult<DmoShortDto[]>> RemoveDmo([FromQuery]RemoveDmoDto dto) {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            var user = await _currentUserService.GetAsync();

            var dmo = await _dmosRepository.GetShortDmo(user.Id, dto.DmoId);
            if (dmo == null) {
                return NotFound();
            }

            _dmosRepository.RemoveDmo(dmo);
            return NoContent();
        }
    }
}