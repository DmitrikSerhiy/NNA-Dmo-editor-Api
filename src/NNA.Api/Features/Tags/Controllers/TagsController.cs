﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NNA.Api.Attributes;
using NNA.Domain.DTOs.Tags;
using NNA.Domain.Interfaces.Repositories;

namespace NNA.Api.Features.Tags.Controllers;


[Route("api/[controller]")]
[ApiController]
public sealed class TagsController : NnaController {
    private readonly ITagsRepository _tagsRepository;
    private readonly IMapper _mapper;

    public TagsController(
        ITagsRepository tagsRepository, 
        IMapper mapper) {
        _tagsRepository = tagsRepository ?? throw new ArgumentNullException(nameof(tagsRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
    
    // todo: add cache 
    [HttpGet]
    [ActiveUserAuthorize]
    public async Task<IActionResult> GetAllTags(CancellationToken cancellationToken) {
        var tags = await _tagsRepository.GetAllTagsWithoutDescriptionAsync(cancellationToken);
        return OkWithData(tags.Select(_mapper.Map<TagWithoutDescriptionDto>));
    }
    
    // todo: add cache 
    [HttpGet("{id}")]
    [NotActiveUserAuthorize]
    public async Task<IActionResult> GetTagDescription([FromRoute] string id, CancellationToken cancellationToken) {
        var tag = await _tagsRepository.GetTagAsync(Guid.Parse(id),  cancellationToken);
        return OkWithData(_mapper.Map<TagDto>(tag));
    }
}