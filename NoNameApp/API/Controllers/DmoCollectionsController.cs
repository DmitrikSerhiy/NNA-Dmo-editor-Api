using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
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
    public class DmoCollectionsController: ControllerBase {

        private readonly IDmoCollectionsRepository _dmoCollectionsRepository;
        private readonly CurrentUserService _currentUserService;
        private readonly ResponseBuilder _responseBuilder;
        private readonly IMapper _mapper;

        public DmoCollectionsController(
            CurrentUserService currentUserService, 
            ResponseBuilder responseBuilder,
            IMapper mapper, 
            IDmoCollectionsRepository dmoCollectionsRepository) {
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            _responseBuilder = responseBuilder ?? throw new ArgumentNullException(nameof(responseBuilder));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _dmoCollectionsRepository = dmoCollectionsRepository ?? throw new ArgumentNullException(nameof(dmoCollectionsRepository));
        }

        [HttpGet]
        [Route("")]
        public async Task<ActionResult<DmoCollectionShortDto[]>> Get() {
            var user = await _currentUserService.GetAsync();
            var dmoCollections = await _dmoCollectionsRepository.GetCollectionsAsync(user.Id);
            return Ok(dmoCollections.Select(_mapper.Map<DmoCollectionShortDto>).ToArray());
        }

        [HttpGet]
        [Route("{collectionId}")]
        public async Task<ActionResult<DmoCollectionDto>> Get(Guid collectionId)
        {
            var user = await _currentUserService.GetAsync();
            var dmoCollection = await _dmoCollectionsRepository.GetCollectionWithDmos(user.Id, collectionId);
            if (dmoCollection == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<DmoCollectionDto>(dmoCollection));
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<DmoCollectionShortDto[]>> Add(DmoCollectionShortDto dmoCollectionShort) {
            var user = await _currentUserService.GetAsync();

            if (await _dmoCollectionsRepository.IsCollectionExist(dmoCollectionShort.CollectionName, user.Id)) {
                return BadRequest(_responseBuilder.AppendBadRequestErrorMessage($"List with name '{dmoCollectionShort.CollectionName}' is already exist"));
            }

            await _dmoCollectionsRepository.AddCollectionAsync(new UserDmoCollection {
                NoNameUser = user,
                NoNameUserId = user.Id,
                CollectionName = dmoCollectionShort.CollectionName
            });

            return NoContent();
        }

        [HttpPut]
        [Route("")]
        public async Task<IActionResult> Update(DmoCollectionShortDto dmoCollectionShort) {
            var user = await _currentUserService.GetAsync();

            var collectionForUpdate = await _dmoCollectionsRepository.GetCollection(dmoCollectionShort.Id, user.Id);
            if (collectionForUpdate == null) {
                return BadRequest(_responseBuilder.AppendBadRequestErrorMessage($"'{dmoCollectionShort.CollectionName}' has been removed or invalid"));
            }

            _dmoCollectionsRepository.UpdateCollectionName(collectionForUpdate, _mapper.Map<UserDmoCollection>(dmoCollectionShort));
            return NoContent();
        }


        [HttpDelete]
        [Route("")]
        public async Task<ActionResult<DmoCollectionShortDto>> Delete(Guid collectionId) {
            var user = await _currentUserService.GetAsync();
            var dmoCollection = await _dmoCollectionsRepository.GetCollection(collectionId, user.Id);
            if (dmoCollection == null) {
                return NotFound();
            }

            _dmoCollectionsRepository.DeleteCollection(dmoCollection);
            return NoContent();
        }


        [HttpDelete]
        [Route("{collectionId}/{dmoId}")]
        public async Task<IActionResult> RemoveDmoFromCollection(Guid collectionId, Guid dmoId)
        {
            var user = await _currentUserService.GetAsync();
            var dmoCollection = await _dmoCollectionsRepository.GetCollectionWithDmos(user.Id, collectionId);
            if (dmoCollection == null)
            {
                return NotFound();
            }

            var dmo = await _dmoCollectionsRepository.GetDmoAsync(user.Id, dmoId);
            if (dmo == null)
            {
                return NotFound();
            }

            if (dmoCollection.Dmos.All(d => d.Id != dmo.Id))
            {
                return NotFound();
            }

            _dmoCollectionsRepository.DeleteDmoFromCollection(dmo);
            return NoContent();
        }
    }
}
