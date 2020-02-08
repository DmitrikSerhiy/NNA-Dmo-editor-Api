using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading;
//using System.Text.Json;
using System.Threading.Tasks;
using API.DTO;
using API.Helpers;
using API.Infrastructure.Authentication;
using AutoMapper;
using Model;
using Model.Entities;

namespace API.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DmoListController: ControllerBase {

        private readonly IDmoCollectionRepository _dmoCollectionRepository;
        private readonly CurrentUserService _currentUserService;
        private readonly ResponseBuilder _responseBuilder;
        private readonly IMapper _mapper;

        public DmoListController(
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
        [Route("all")]
        public async Task<ActionResult<DmoListDto[]>> GetAll() {
            var user = await _currentUserService.GetAsync();
            var dmoCollections = await _dmoCollectionRepository.GetAllAsync(user.Id);
            //Thread.Sleep(1500);
            return Ok(dmoCollections.Select(_mapper.Map<DmoListDto>).ToArray());
        }

        [HttpGet]
        [Route("")]
        public async Task<ActionResult<DmoListDto>> Get(Guid collectionId) {
            var user = await _currentUserService.GetAsync();
            var dmoCollection = await _dmoCollectionRepository.Get(collectionId, user.Id);
            if (dmoCollection == null) {
                return NotFound();
            }
            return Ok(_mapper.Map<DmoListDto>(dmoCollection));
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<DmoListDto[]>> Add(DmoListDto dmoList) {
            var user = await _currentUserService.GetAsync();

            if (await _dmoCollectionRepository.IsExist(dmoList.CollectionName, user.Id)) {
                return BadRequest(_responseBuilder.AppendBadRequestErrorMessage($"List with name '{dmoList.CollectionName}' is already exist"));
            }

            await _dmoCollectionRepository.AddAsync(new UserDmoCollection {
                NoNameUser = user,
                NoNameUserId = user.Id,
                CollectionName = dmoList.CollectionName
            });

            //Thread.Sleep(1500);

            return NoContent();
        }

        [HttpPut]
        [Route("")]
        public async Task<IActionResult> Update(DmoListDto dmoList) {
            var user = await _currentUserService.GetAsync();

            var collectionForUpdate = await _dmoCollectionRepository.Get(dmoList.Id, user.Id);
            if (collectionForUpdate == null) {
                return BadRequest(_responseBuilder.AppendBadRequestErrorMessage($"'{dmoList.CollectionName}' has been removed or invalid"));
            }

            _dmoCollectionRepository.Update(collectionForUpdate, _mapper.Map<UserDmoCollection>(dmoList));
            return NoContent();
        }


        [HttpDelete]
        [Route("")]
        public async Task<ActionResult<DmoListDto>> Delete(Guid collectionId) {
            var user = await _currentUserService.GetAsync();
            var dmoCollection = await _dmoCollectionRepository.Get(collectionId, user.Id);
            if (dmoCollection == null) {
                return NotFound();
            }

            _dmoCollectionRepository.Delete(dmoCollection);

            return Ok();
        }
    }
}
