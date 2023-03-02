using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NNA.Api.Attributes;
using NNA.Domain.DTOs;
using NNA.Domain.DTOs.Community;
using NNA.Domain.Interfaces.Repositories;

namespace NNA.Api.Features.Community.Controllers;

[Route("api/[controller]")]
[ApiController]
[NotActiveUserAuthorize]
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

    // todo: escape or encode searchBy param (by SQL language) to prevent any kind of SQL injection
    [HttpPost("dmos/search/amount")]
    public async Task<IActionResult> SearchPublishedDmoAmount([FromQuery] string searchBy, [FromBody] SearchPublishedDmoAmountDto searchPublishedDmoAmountDto, CancellationToken token) {
        if (string.IsNullOrWhiteSpace(searchBy)) {
            return OkWithData(0);
        }
        
        var dmosToIgnore = new List<Guid>();
        if (searchPublishedDmoAmountDto.DmoIdsToIgnore.Length > 0) {
            dmosToIgnore.AddRange(searchPublishedDmoAmountDto.DmoIdsToIgnore.Select(Guid.Parse));
        }

        var searchedAmount = await _communityRepository.GetPublishedDmosAmountAsync(dmosToIgnore, searchBy, token);
        return OkWithData(searchedAmount);
    }
    
    // todo: escape or encode searchBy param  (by SQL language) to prevent any kind of SQL injection
    [HttpPost("dmos/search/data")]
    public async Task<IActionResult> SearchPublishedDmoData([FromQuery] string searchBy, [FromBody] SearchPublishedDmosDto searchPublishedDmosDto, CancellationToken token) {
        var dmosToIgnore = new List<Guid>();
        if (searchPublishedDmosDto.DmoIdsToIgnore.Length > 0) {
            dmosToIgnore.AddRange(searchPublishedDmosDto.DmoIdsToIgnore.Select(Guid.Parse));
        }

        var searchedDmos = await _communityRepository.GetPublishedDmosAsync(dmosToIgnore, searchBy, searchPublishedDmosDto.Amount, token);
        var publishedDmoDtos = searchedDmos.Select(_mapper.Map<PublishedDmoShortDto>).ToList();
        return OkWithData(publishedDmoDtos);
    }
}