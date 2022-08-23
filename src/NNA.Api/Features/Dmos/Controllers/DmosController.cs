using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.DTOs.DmoCollections;
using Model.DTOs.Dmos;
using Model.Interfaces;
using Model.Interfaces.Repositories;

namespace NNA.Api.Features.Dmos.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class DmosController : ControllerBase {
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
    [Route("")]
    public async Task<ActionResult<DmoShortDto[]>> GetDmos() {
        var dmos = await _dmosRepository.GetAll(_authenticatedIdentityProvider.AuthenticatedUserId);
        return Ok(dmos.Select(_mapper.Map<DmoShortDto>).ToArray());
    }

    [HttpDelete]
    [Route("")]
    public async Task<ActionResult<DmoShortDto[]>> RemoveDmo([FromQuery] RemoveDmoDto dto) {
        if (dto == null) throw new ArgumentNullException(nameof(dto));

        var dmo = await _dmosRepository.GetDmo(_authenticatedIdentityProvider.AuthenticatedUserId, dto.DmoId);
        if (dmo == null) {
            return NotFound();
        }

        _dmosRepository.DeleteDmo(dmo);
        return NoContent();
    }
}