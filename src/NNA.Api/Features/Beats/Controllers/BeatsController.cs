﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NNA.Domain.DTOs.Beats;
using NNA.Domain.Interfaces;
using NNA.Domain.Interfaces.Repositories;

namespace NNA.Api.Features.Beats.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class BeatsController : NnaController {
    private readonly IDmosRepository _dmosRepository;
    private readonly IAuthenticatedIdentityProvider _authenticatedIdentityProvider;
    private readonly IMapper _mapper;

    public BeatsController(
        IDmosRepository dmosRepository,
        IAuthenticatedIdentityProvider authenticatedIdentityProvider,
        IMapper mapper) {
        _dmosRepository = dmosRepository ?? throw new ArgumentNullException(nameof(dmosRepository));
        _authenticatedIdentityProvider = authenticatedIdentityProvider
                                         ?? throw new ArgumentNullException(nameof(authenticatedIdentityProvider));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    [HttpGet]
    [Route("initial/array/{dmoId}")]
    public async Task<IActionResult> InitialLoadAsArray(Guid dmoId, CancellationToken cancellationToken) {
        var beats = await _dmosRepository.GetBeatsForDmoAsync(_authenticatedIdentityProvider.AuthenticatedUserId, dmoId, cancellationToken);
        return OkWithData(beats.Select(_mapper.Map<BeatDto>).ToArray());
    }
}