﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NNA.Domain.DTOs;
using NNA.Domain.DTOs.Community;
using NNA.Domain.Interfaces.Repositories;

namespace NNA.Api.Features.Community.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public sealed class CommunityController : NnaController {

    private readonly ICommunityRepository _communityRepository;
    private readonly IMapper _mapper;

    public CommunityController(
        ICommunityRepository communityRepository, 
        IMapper mapper) {
        _communityRepository = communityRepository ?? throw new ArgumentNullException(nameof(communityRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    [HttpGet("dmos")]
    public async Task<IActionResult> GetPublishedDmos([FromQuery] PaginationDetailsDto paginationDetailsDto, CancellationToken token) {
        var totalAmount = await _communityRepository.GetPublishedAmountAsync(token);
        if (totalAmount == 0) {
            return OkWithData(new PublishedDmosDto());
        }

        var publishedDmos = await _communityRepository.GetPublishedDmosAsync(
            paginationDetailsDto.PageSize, 
            paginationDetailsDto.PageNumber,
            token);

        var publishedDmoDtos = publishedDmos.Select(_mapper.Map<PublishedDmoShortDto>).ToList();
        var result = new PublishedDmosDto {
            Dmos = publishedDmoDtos,
            Pagination = new PaginationDetailsResultDto(totalAmount, paginationDetailsDto.PageNumber, paginationDetailsDto.PageSize)
        };

        return OkWithData(result);
    }
    
    [HttpGet("dmos/{id}/details")]
    public async Task<IActionResult> GetPublishedDmoMetadata([FromRoute] string id, CancellationToken token) {
        var dmoId = Guid.Parse(id);
        var beatsCount = await _communityRepository.GetBeatsCount(dmoId, token);
        var realBeatsCount = await _communityRepository.GetNonAestheticBeatsCount(dmoId, token);
        var charactersCount = await _communityRepository.GetCharactersCount(dmoId, token);
        var (premise, controllingIdea) = await _communityRepository.GetDmoPremiseAndControllingIdea(dmoId, token);

        var detailsDto = new PublishedDmoDetailsDto {
            Id = id,
            BeatsCount = realBeatsCount,
            CharactersCount = charactersCount,
            Premise = premise,
            ControllingIdea = controllingIdea,
            MinutesToRead = beatsCount <= 14 ? 1 : (int)Math.Ceiling(beatsCount / (double)14)
        }; // ~14 beats takes 1 minute to read
        
        return OkWithData(detailsDto);
    }
}