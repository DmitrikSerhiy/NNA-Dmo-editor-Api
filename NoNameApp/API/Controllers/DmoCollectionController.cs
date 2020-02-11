using System;
using System.Linq;
using System.Threading.Tasks;
using API.DTO;
using API.Helpers;
using API.Infrastructure.Authentication;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DmoCollectionController: ControllerBase {
        private readonly IDmoCollectionRepository _dmoCollectionRepository;
        private readonly CurrentUserService _currentUserService;
        private readonly ResponseBuilder _responseBuilder;
        private readonly IMapper _mapper;

        public DmoCollectionController(
            IDmoCollectionRepository dmoCollectionRepository, 
            CurrentUserService currentUserService, 
            ResponseBuilder responseBuilder,
            IMapper mapper) {
            _dmoCollectionRepository = dmoCollectionRepository ?? throw new ArgumentNullException(nameof(dmoCollectionRepository));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            _responseBuilder = responseBuilder ?? throw new ArgumentNullException(nameof(responseBuilder));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        [Route("{collectionId}")]
        public async Task<ActionResult<DmoCollectionDto>> Get(Guid collectionId) {
            var user = await _currentUserService.GetAsync();
            var dmoCollection = await _dmoCollectionRepository.GetAllDmoAsync(user.Id, collectionId);
            if (dmoCollection == null) {
                return NotFound();
            }
            return Ok(dmoCollection.Select(_mapper.Map<DmoCollectionDto>).ToArray());
        }

        //[HttpPut]
        //[Route("")]
        //public async Task<ActionResult<DmoCollectionDto>> AddDmo(Guid collectionId)
        //{
        //    var user = await _currentUserService.GetAsync();
        //    var dmoCollection = await _dmoCollectionRepository.GetAllDmoAsync(user.Id, collectionId);
        //    if (dmoCollection == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(dmoCollection.Select(_mapper.Map<DmoCollectionDto>).ToArray());
        //}

        //[HttpDelete]
        //[Route("{collectionId}")]
        //public async Task<ActionResult<DmoCollectionDto>> Get(Guid collectionId)
        //{
        //    var user = await _currentUserService.GetAsync();
        //    var dmoCollection = await _dmoCollectionRepository.GetAllDmoAsync(user.Id, collectionId);
        //    if (dmoCollection == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(dmoCollection.Select(_mapper.Map<DmoCollectionDto>).ToArray());
        //}

    }
}
