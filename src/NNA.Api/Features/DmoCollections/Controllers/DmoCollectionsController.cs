using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NNA.Api.Helpers;
using NNA.Domain.DTOs.DmoCollections;
using NNA.Domain.Entities;
using NNA.Domain.Interfaces;
using NNA.Domain.Interfaces.Repositories;

namespace NNA.Api.Features.DmoCollections.Controllers; 

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class DmoCollectionsController: ControllerBase {

    private readonly IDmoCollectionsRepository _dmoCollectionsRepository;
    private readonly IAuthenticatedIdentityProvider _authenticatedIdentityProvider;
    private readonly IMapper _mapper;

    public DmoCollectionsController(
        IMapper mapper, 
        IDmoCollectionsRepository dmoCollectionsRepository, 
        IAuthenticatedIdentityProvider authenticatedIdentityProvider) {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _dmoCollectionsRepository = dmoCollectionsRepository 
                                    ?? throw new ArgumentNullException(nameof(dmoCollectionsRepository));
        _authenticatedIdentityProvider = authenticatedIdentityProvider
                                         ?? throw new ArgumentNullException(nameof(authenticatedIdentityProvider));
    }

    #region collections

    [HttpGet]
    [Route("")]
    public async Task<ActionResult<DmoCollectionShortDto[]>> GetCollections() {
        var dmoCollections = await _dmoCollectionsRepository
            .GetCollectionsAsync(_authenticatedIdentityProvider.AuthenticatedUserId);
        return Ok(dmoCollections.Select(_mapper.Map<DmoCollectionShortDto>).ToArray());
    }

    [HttpPost]
    [Route("")]
    public async Task<ActionResult<DmoCollectionShortDto[]>> AddCollection(AddNewDmoCollectionDto dto) {
        if (dto == null) throw new ArgumentNullException(nameof(dto));

        if (await _dmoCollectionsRepository.IsCollectionExist(_authenticatedIdentityProvider.AuthenticatedUserId, dto.CollectionName)) {
            return BadRequest(ResponseBuilder.AppendBadRequestErrorMessage($"List with name '{dto.CollectionName}' is already exist"));
        }

        await _dmoCollectionsRepository.AddCollectionAsync(new DmoCollection {
            NnaUserId = _authenticatedIdentityProvider.AuthenticatedUserId,
            CollectionName = dto.CollectionName
        });

        return NoContent();
    }

    [HttpDelete]
    [Route("")]
    public async Task<ActionResult<DmoCollectionShortDto>> DeleteCollection([FromQuery]DeleteCollectionDto dto) {
        if (dto == null) throw new ArgumentNullException(nameof(dto));
        var dmoCollection = await _dmoCollectionsRepository
            .GetCollectionWithDmos(_authenticatedIdentityProvider.AuthenticatedUserId, dto.CollectionId);
        if (dmoCollection is null) {
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
        var dmoCollection = await _dmoCollectionsRepository
            .GetCollectionAsync(_authenticatedIdentityProvider.AuthenticatedUserId, dto.CollectionId);
        if (dmoCollection is null) {
            return NotFound();
        }

        return Ok(_mapper.Map<DmoCollectionShortDto>(dmoCollection));
    }

    [HttpPut]
    [Route("collection/name")]
    public async Task<IActionResult> UpdateCollectionName(DmoCollectionShortDto dmoCollectionShort) {
        if (dmoCollectionShort == null) throw new ArgumentNullException(nameof(dmoCollectionShort));

        var collectionForUpdate = await _dmoCollectionsRepository
            .GetCollectionAsync(_authenticatedIdentityProvider.AuthenticatedUserId, dmoCollectionShort.Id);
        if (collectionForUpdate is null) {
            return BadRequest(ResponseBuilder.AppendBadRequestErrorMessage($"'{dmoCollectionShort.CollectionName}' has been removed or invalid"));
        }

        _dmoCollectionsRepository.UpdateCollectionName(collectionForUpdate, _mapper.Map<DmoCollection>(dmoCollectionShort));
        return NoContent();
    }

    #endregion


    #region collectionWithDmos

    [HttpGet]
    [Route("collection")]
    public async Task<ActionResult<DmoCollectionDto>> GetCollection([FromQuery]GetCollectionDto dto) {
        if (dto == null) throw new ArgumentNullException(nameof(dto));

        var dmoCollection = await _dmoCollectionsRepository
            .GetCollectionWithDmos(_authenticatedIdentityProvider.AuthenticatedUserId, dto.CollectionId);
        if (dmoCollection is null) {
            return NotFound();
        }

        return Ok(_mapper.Map<DmoCollectionDto>(dmoCollection));
    }

    [HttpGet]
    [Route("collection/dmos")]
    public async Task<ActionResult<DmoShortDto[]>> GetExcludedDmos([FromQuery]GetExcludedDmosDto dto) {
        if (dto is null) throw new ArgumentNullException(nameof(dto));

        var dmos = await _dmoCollectionsRepository
            .GetExcludedDmos(_authenticatedIdentityProvider.AuthenticatedUserId, dto.CollectionId);

        return Ok(dmos.Select(_mapper.Map<DmoShortDto>).ToArray());
    }

    [HttpPost]
    [Route("collection/dmos")]
    public async Task<IActionResult> AddDmoToCollection([FromBody]AddDmoToCollectionDto dto) {
        if (dto == null) throw new ArgumentNullException(nameof(dto));
        var dmoCollection = await _dmoCollectionsRepository
            .GetCollectionWithDmos(_authenticatedIdentityProvider.AuthenticatedUserId, dto.CollectionId);
        if (dmoCollection is null) {
            return NotFound();
        }

        foreach (var dmoInCollection in dto.Dmos) {
            if (_dmoCollectionsRepository.ContainsDmo(dmoCollection, dmoInCollection.Id)) {
                return BadRequest(ResponseBuilder.AppendBadRequestErrorMessage($"Collection already contains dmo with id {dmoInCollection.Id}"));
            }
        }

        var dmos = new List<Dmo>();
        foreach (var dmoInCollection in dto.Dmos) {
            var dmo = await _dmoCollectionsRepository
                .GetDmoAsync(_authenticatedIdentityProvider.AuthenticatedUserId, dmoInCollection.Id);
            if (dmo == null) {
                return NotFound();
            }
            dmos.Add(dmo);
        }

        _dmoCollectionsRepository.AddDmoToCollection(dmoCollection, dmos);
        return NoContent();
    }


    [HttpDelete]
    [Route("collection/dmos")]
    public async Task<IActionResult> RemoveDmoFromCollection([FromQuery]RemoveDmoFromCollectionDto dto) {
        if (dto == null) throw new ArgumentNullException(nameof(dto));
        var dmoCollection = await _dmoCollectionsRepository
            .GetCollectionWithDmos(_authenticatedIdentityProvider.AuthenticatedUserId, dto.CollectionId);
        if (dmoCollection is null) {
            return NotFound();
        }

        var dmo = await _dmoCollectionsRepository.GetDmoAsync(_authenticatedIdentityProvider.AuthenticatedUserId, dto.DmoId);
        if (dmo is null) {
            return NotFound();
        }

        if (dmoCollection.DmoCollectionDmos.All(d => d.Dmo!.Id != dmo.Id)) {
            return NotFound();
        }

        _dmoCollectionsRepository.RemoveDmoFromCollection(dmoCollection, dmo);
        return NoContent();
    }

    #endregion
}