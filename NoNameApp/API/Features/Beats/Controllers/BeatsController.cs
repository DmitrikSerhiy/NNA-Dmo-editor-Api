using System;
using System.Threading.Tasks;
using API.Features.Account.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.DTOs.Beats;
using Model.Interfaces.Repositories;

namespace API.Features.Beats.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BeatsController : ControllerBase {

        private readonly IDmosRepository _dmosRepository; //todo: move to beats repository with diff lib
        private readonly CurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public BeatsController(
            IDmosRepository dmosRepository, 
            CurrentUserService currentUserService, 
            IMapper mapper) {
            _dmosRepository = dmosRepository ?? throw new ArgumentNullException(nameof(dmosRepository));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }


        [HttpGet]
        [Route("initial/{dmoId}")]
        public async Task<DmoWithBeatsJsonDto> InitialLoad(Guid dmoId) {
            var user = await _currentUserService.GetAsync();

            var dmo = await _dmosRepository.GetDmoWithBeatsJson(user.Id, dmoId);

            return _mapper.Map<DmoWithBeatsJsonDto>(dmo);
        }
    }
}
