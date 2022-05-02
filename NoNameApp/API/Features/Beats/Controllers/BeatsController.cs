using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.DTOs.Beats;
using Model.Interfaces;
using Model.Interfaces.Repositories;

namespace API.Features.Beats.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BeatsController : ControllerBase {

        private readonly IDmosRepository _dmosRepository; // todo: move to beats repository with diff lib
        private readonly IAuthenticatedIdentityProvider _authenticatedIdentityProvider;
        private readonly IMapper _mapper;

        public BeatsController(
            IDmosRepository dmosRepository, 
            IAuthenticatedIdentityProvider authenticatedIdentityProvider, 
            IMapper mapper) {
            _dmosRepository = dmosRepository ?? throw new ArgumentNullException(nameof(dmosRepository));
            _authenticatedIdentityProvider = authenticatedIdentityProvider 
                                             ?? throw new ArgumentNullException(nameof(authenticatedIdentityProvider));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }


        [HttpGet]
        [Route("initial/json/{dmoId}")]
        public async Task<DmoWithBeatsJsonDto> InitialLoadAsJson(Guid dmoId) {
            var dmo = await _dmosRepository.GetDmoWithBeatsJson(_authenticatedIdentityProvider.AuthenticatedUserId, dmoId);

            return _mapper.Map<DmoWithBeatsJsonDto>(dmo);
        }
        
        [HttpGet]
        [Route("initial/array/{dmoId}")]
        public async Task<IActionResult> InitialLoadAsArray(Guid dmoId) {
            var beats = await _dmosRepository.GetBeatsForDmo(_authenticatedIdentityProvider.AuthenticatedUserId, dmoId);
            return Ok(beats.Select(_mapper.Map<BeatDto>).ToArray());
        }
    }
}
