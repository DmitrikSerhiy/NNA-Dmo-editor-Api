using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTO.DmoCollections;
using API.Helpers;
using API.Infrastructure.Authentication;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace API.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DmosController : ControllerBase {
        private readonly CurrentUserService _currentUserService;
        private readonly ResponseBuilder _responseBuilder;
        private readonly IMapper _mapper;
        private readonly IDmosRepository _dmosRepository;

        public DmosController(
            CurrentUserService currentUserService, 
            ResponseBuilder responseBuilder, 
            IMapper mapper, 
            IDmosRepository dmosRepository) {
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            _responseBuilder = responseBuilder ?? throw new ArgumentNullException(nameof(responseBuilder));
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

    }
}