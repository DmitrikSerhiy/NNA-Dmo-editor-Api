using System;
using System.Threading.Tasks;
using API.Features.Account.Services;
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

        public BeatsController(
            IDmosRepository dmosRepository, 
            CurrentUserService currentUserService) {
            _dmosRepository = dmosRepository ?? throw new ArgumentNullException(nameof(dmosRepository));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        }


        [HttpGet]
        [Route("initial/{dmoId}")]
        public async Task<BeatsDto> InitialLoad(Guid dmoId) {
            var user = await _currentUserService.GetAsync();

            var beatsJson = await _dmosRepository.GetBeatsJson(user.Id, dmoId);

            return new BeatsDto { DmoId = dmoId, BeatsJson = beatsJson };
        }
    }
}
