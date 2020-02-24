using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using API.DTO.DmoCollections;
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

        #region collections

        [HttpGet]
        [Route("")]
        public async Task<ActionResult<DmoCollectionShortDto[]>> GetCollections() {
            var user = await _currentUserService.GetAsync();
            var dmoCollections = await _dmoCollectionsRepository.GetCollectionsAsync(user.Id);
            return Ok(dmoCollections.Select(_mapper.Map<DmoCollectionShortDto>).ToArray());
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<DmoCollectionShortDto[]>> AddCollection(AddNewDmoCollectionDto dto) {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            var user = await _currentUserService.GetAsync();

            if (await _dmoCollectionsRepository.IsCollectionExist(user.Id, dto.CollectionName))
            {
                return BadRequest(_responseBuilder.AppendBadRequestErrorMessage($"List with name '{dto.CollectionName}' is already exist"));
            }

            await _dmoCollectionsRepository.AddCollectionAsync(new UserDmoCollection
            {
                NoNameUser = user,
                NoNameUserId = user.Id,
                CollectionName = dto.CollectionName
            });

            return NoContent();
        }

        [HttpDelete]
        [Route("")]
        public async Task<ActionResult<DmoCollectionShortDto>> DeleteCollection([FromQuery]DeleteCollectionDto dto) {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            var user = await _currentUserService.GetAsync();
            var dmoCollection = await _dmoCollectionsRepository.GetCollectionWithDmos(user.Id, dto.CollectionId);
            if (dmoCollection == null)
            {
                return NotFound();
            }

            _dmoCollectionsRepository.DeleteCollection(dmoCollection);
            return NoContent();
        }

        #endregion


        #region collectionName

        [HttpGet]
        [Route("collection/name")]
        public async Task<ActionResult<DmoCollectionShortDto>> GetCollectionName([FromQuery]CollectionNameDto dto) {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            var user = await _currentUserService.GetAsync();
            var dmoCollection = await _dmoCollectionsRepository.GetCollectionAsync(user.Id, dto.CollectionId);
            if (dmoCollection == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<DmoCollectionShortDto>(dmoCollection));
        }

        [HttpPut]
        [Route("collection/name")]
        public async Task<IActionResult> UpdateCollectionName(DmoCollectionShortDto dmoCollectionShort) {
            if (dmoCollectionShort == null) throw new ArgumentNullException(nameof(dmoCollectionShort));
            var user = await _currentUserService.GetAsync();

            var collectionForUpdate = await _dmoCollectionsRepository.GetCollectionAsync(user.Id, dmoCollectionShort.Id);
            if (collectionForUpdate == null) {
                return BadRequest(_responseBuilder.AppendBadRequestErrorMessage($"'{dmoCollectionShort.CollectionName}' has been removed or invalid"));
            }

            _dmoCollectionsRepository.UpdateCollectionName(collectionForUpdate, _mapper.Map<UserDmoCollection>(dmoCollectionShort));
            return NoContent();
        }

        #endregion


        #region collectionWithDmos

        [HttpGet]
        [Route("collection")]
        public async Task<ActionResult<DmoCollectionDto>> GetCollection([FromQuery]GetCollectionDto dto) {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var user = await _currentUserService.GetAsync();
            var dmoCollection = await _dmoCollectionsRepository.GetCollectionWithDmos(user.Id, dto.CollectionId);
            if (dmoCollection == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<DmoCollectionDto>(dmoCollection));
        }

        [HttpPost]
        [Route("collection/dmos")]
        public async Task<IActionResult> AddDmoToCollection(AddDmoToCollectionDto dto) {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            var user = await _currentUserService.GetAsync();
            var dmoCollection = await _dmoCollectionsRepository.GetCollectionWithDmos(user.Id, dto.CollectionId);
            if (dmoCollection == null) {
                return NotFound();
            }

            if (_dmoCollectionsRepository.ContainsDmo(dmoCollection, dto.DmoId)) {
                return BadRequest(_responseBuilder.AppendBadRequestErrorMessage($"Collection already contains dmo with id {dto.DmoId}"));
            }

            var dmo = await _dmoCollectionsRepository.GetDmoAsync(user.Id, dto.DmoId);
            if (dmo == null) {
                return NotFound();
            }

            _dmoCollectionsRepository.AddDmoToCollection(dmoCollection, dmo);
            return NoContent();
        }


        [HttpDelete]
        [Route("collection/dmos")]
        public async Task<IActionResult> RemoveDmoFromCollection([FromQuery]RemoveDmoFromCollectionDto dto) {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            var user = await _currentUserService.GetAsync();
            var dmoCollection = await _dmoCollectionsRepository.GetCollectionWithDmos(user.Id, dto.CollectionId);
            if (dmoCollection == null) {
                return NotFound();
            }

            var dmo = await _dmoCollectionsRepository.GetDmoAsync(user.Id, dto.DmoId);
            if (dmo == null) {
                return NotFound();
            }

            if (dmoCollection.DmoUserDmoCollections.All(d => d.Dmo.Id != dmo.Id)) {
                return NotFound();
            }

            _dmoCollectionsRepository.RemoveDmoFromCollection(dmoCollection, dmo);
            return NoContent();
        }

        #endregion
    }
}
