using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NNA.Domain.DTOs.DmoCollections;
using NNA.Domain.DTOs.Dmos;
using NNA.Domain.Interfaces;
using NNA.Domain.Interfaces.Repositories;

namespace NNA.Api.Features.Dmos.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class DmosController : NnaController {
    private readonly IMapper _mapper;
    private readonly IAuthenticatedIdentityProvider _authenticatedIdentityProvider;
    private readonly IDmosRepository _dmosRepository;

    public DmosController(
        IMapper mapper,
        IDmosRepository dmosRepository,
        IAuthenticatedIdentityProvider authenticatedIdentityProvider) {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _dmosRepository = dmosRepository ?? throw new ArgumentNullException(nameof(dmosRepository));
        _authenticatedIdentityProvider = authenticatedIdentityProvider
                                         ?? throw new ArgumentNullException(nameof(authenticatedIdentityProvider));
    }

    [HttpGet]
    public async Task<IActionResult> GetDmos() {
        var dmos = await _dmosRepository.GetAll(_authenticatedIdentityProvider.AuthenticatedUserId);
        return OkWithData(dmos.Select(_mapper.Map<DmoShortDto>).ToArray());
    }

    [HttpDelete]
    public async Task<IActionResult> RemoveDmo([FromQuery] RemoveDmoDto dto) {
        if (dto == null) throw new ArgumentNullException(nameof(dto));

        var dmo = await _dmosRepository.GetDmo(_authenticatedIdentityProvider.AuthenticatedUserId, dto.DmoId);
        if (dmo == null) {
            return BadRequestWithMessageToToastr($"Dmo {dto.DmoId} is not found");
        }

        _dmosRepository.DeleteDmo(dmo);
        return NoContent();
    }
}