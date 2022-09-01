﻿using AutoMapper;
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
public class DmoCollectionsController : NnaController {
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
    public async Task<IActionResult> GetCollections() {
        var dmoCollections = await _dmoCollectionsRepository
            .GetCollectionsAsync(_authenticatedIdentityProvider.AuthenticatedUserId);
        return OkWithData(dmoCollections.Select(_mapper.Map<DmoCollectionShortDto>).ToArray());
    }

    [HttpPost]
    public async Task<IActionResult> AddCollection(AddNewDmoCollectionDto dto) {
        if (await _dmoCollectionsRepository.IsCollectionExist(_authenticatedIdentityProvider.AuthenticatedUserId,
                dto.CollectionName)) {
            return BadRequestWithMessageToToastr($"List with name '{dto.CollectionName}' is already exist");
        }
    
        await _dmoCollectionsRepository.AddCollectionAsync(new DmoCollection {
            NnaUserId = _authenticatedIdentityProvider.AuthenticatedUserId,
            CollectionName = dto.CollectionName
        });
    
        return NoContent();
    }
    
    [HttpDelete]
    public async Task<IActionResult> DeleteCollection([FromQuery] DeleteCollectionDto dto) {
        var dmoCollection = await _dmoCollectionsRepository
            .GetCollectionWithDmos(_authenticatedIdentityProvider.AuthenticatedUserId, dto.CollectionId);
        if (dmoCollection is null) {
            return BadRequestWithMessageToToastr($"Collection '{dto.CollectionId}' is not found");
        }
    
        _dmoCollectionsRepository.DeleteCollection(dmoCollection);
        return NoContent();
    }
    
    #endregion
    
    
    #region collectionName
    
    [HttpGet]
    [Route("collection/name")]
    public async Task<IActionResult> GetCollectionName([FromQuery] CollectionNameDto dto) {
        var dmoCollection = await _dmoCollectionsRepository
            .GetCollectionAsync(_authenticatedIdentityProvider.AuthenticatedUserId, dto.CollectionId);
        if (dmoCollection is null) {
            return BadRequestWithMessageToToastr($"Failed to find collection");
        }
    
        return OkWithData(_mapper.Map<DmoCollectionShortDto>(dmoCollection));
    }
    
    [HttpPut]
    [Route("collection/name")]
    public async Task<IActionResult> UpdateCollectionName(UpdateDmoCollectionNameDto updateDmoCollectionNameDto) {
        var collectionForUpdate = await _dmoCollectionsRepository
            .GetCollectionAsync(_authenticatedIdentityProvider.AuthenticatedUserId, updateDmoCollectionNameDto.Id);
        if (collectionForUpdate is null) {
            return BadRequestWithMessageToToastr($"'{updateDmoCollectionNameDto.CollectionName}' has been removed or invalid");
        }
    
        _dmoCollectionsRepository.UpdateCollectionName(collectionForUpdate,
            _mapper.Map<DmoCollection>(updateDmoCollectionNameDto));
        return NoContent();
    }
    
    #endregion
    
    
    #region collectionWithDmos
    
    [HttpGet]
    [Route("collection")]
    public async Task<IActionResult> GetCollection([FromQuery] GetCollectionDto dto) {
        var dmoCollection = await _dmoCollectionsRepository
            .GetCollectionWithDmos(_authenticatedIdentityProvider.AuthenticatedUserId, dto.CollectionId);
        if (dmoCollection is null) {
            return BadRequestWithMessageToToastr("Failed to find collection");
        }
    
        return OkWithData(_mapper.Map<DmoCollectionDto>(dmoCollection));
    }
    
    [HttpGet]
    [Route("collection/dmos")]
    public async Task<IActionResult> GetExcludedDmos([FromQuery] GetExcludedDmosDto dto) {
        var dmos = await _dmoCollectionsRepository
            .GetExcludedDmos(_authenticatedIdentityProvider.AuthenticatedUserId, dto.CollectionId);
    
        return OkWithData(dmos.Select(_mapper.Map<DmoShortDto>).ToArray());
    }
    
    [HttpPost]
    [Route("collection/dmos")]
    public async Task<IActionResult> AddDmoToCollection([FromBody] AddDmoToCollectionDto dto) {
        var dmoCollection = await _dmoCollectionsRepository
            .GetCollectionWithDmos(_authenticatedIdentityProvider.AuthenticatedUserId, dto.CollectionId);
        if (dmoCollection is null) {
            return BadRequestWithMessageToToastr("Failed to find collection");
        }
    
        foreach (var dmoInCollection in dto.Dmos) {
            if (_dmoCollectionsRepository.ContainsDmo(dmoCollection, dmoInCollection.Id)) {
                return BadRequestWithMessageToToastr($"Collection already contains dmo with id {dmoInCollection.Id}");
            }
        }
    
        var dmos = new List<Dmo>();
        foreach (var dmoInCollection in dto.Dmos) {
            var dmo = await _dmoCollectionsRepository
                .GetDmoAsync(_authenticatedIdentityProvider.AuthenticatedUserId, dmoInCollection.Id);
            if (dmo == null) {
                return BadRequestWithMessageToToastr($"Dmo {dmoInCollection.Id} is not found");
            }
    
            dmos.Add(dmo);
        }
    
        _dmoCollectionsRepository.AddDmoToCollection(dmoCollection, dmos);
        return NoContent();
    }
    
    
    [HttpDelete]
    [Route("collection/dmos")]
    public async Task<IActionResult> RemoveDmoFromCollection([FromQuery] RemoveDmoFromCollectionDto dto) {
        var dmoCollection = await _dmoCollectionsRepository
            .GetCollectionWithDmos(_authenticatedIdentityProvider.AuthenticatedUserId, dto.CollectionId);
        if (dmoCollection is null) {
            return BadRequestWithMessageToToastr($"Collection {dto.CollectionId} is not found");
        }
    
        var dmo = await _dmoCollectionsRepository.GetDmoAsync(_authenticatedIdentityProvider.AuthenticatedUserId,
            dto.DmoId);
        if (dmo is null) {
            return BadRequestWithMessageToToastr($"Dmo {dto.DmoId} is not found");
        }
    
        if (dmoCollection.DmoCollectionDmos.All(d => d.Dmo!.Id != dmo.Id)) {
            return NoContent();
        }
    
        _dmoCollectionsRepository.RemoveDmoFromCollection(dmoCollection, dmo);
        return NoContent();
    }
    
    #endregion
}