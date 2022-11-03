
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NNA.Domain.DTOs.Beats;
using NNA.Domain.DTOs.Characters;
using NNA.Domain.DTOs.DmoCollections;
using NNA.Domain.DTOs.Dmos;
using NNA.Domain.DTOs.Editor;
using NNA.Domain.Entities;
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
    private readonly IEditorService _editorService;

    public DmosController(
        IMapper mapper,
        IDmosRepository dmosRepository,
        IEditorService editorService,
        IAuthenticatedIdentityProvider authenticatedIdentityProvider) {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _dmosRepository = dmosRepository ?? throw new ArgumentNullException(nameof(dmosRepository));
        _editorService = editorService ?? throw new ArgumentNullException(nameof(editorService));
        _authenticatedIdentityProvider = authenticatedIdentityProvider ?? throw new ArgumentNullException(nameof(authenticatedIdentityProvider));
    }

    // todo: add pagination here
    [HttpGet]
    public async Task<IActionResult> GetDmos(CancellationToken cancellationToken) {
        var dmos = await _dmosRepository.GetAllAsync(_authenticatedIdentityProvider.AuthenticatedUserId, cancellationToken);
        return OkWithData(dmos.Select(_mapper.Map<DmoShortDto>).ToArray());
    }

    [HttpPost]
    public async Task<IActionResult> CreateDmo(CreateDmoByHttpDto newDmoDto) {
        var dmoDtoForEditor = _mapper.Map<CreateDmoDto>(newDmoDto);
        var newDmo = await _editorService.CreateAndLoadDmo(dmoDtoForEditor, _authenticatedIdentityProvider.AuthenticatedUserId);
        return OkWithData(_mapper.Map<CreatedDmoByHttpDto>(newDmo));
    }

    [HttpDelete]
    public async Task<IActionResult> RemoveDmo([FromQuery] RemoveDmoDto dto, CancellationToken cancellationToken) {
        if (dto == null) throw new ArgumentNullException(nameof(dto));

        var dmo = await _dmosRepository.GetDmoAsync(_authenticatedIdentityProvider.AuthenticatedUserId, dto.DmoId, cancellationToken);
        if (dmo == null) {
            return BadRequestWithMessageToToastr($"Dmo {dto.DmoId} is not found");
        }

        _dmosRepository.DeleteDmo(dmo);
        return NoContent();
    }
    
    [HttpGet]
    [Route("withdata")]
    public async Task<IActionResult> LoadDmoWithData([FromQuery] GetDmoWithDataDto getDmoWithDataDto, CancellationToken cancellationToken) {
        var dmoWithData = await _dmosRepository.GetDmoWithDataAsync(_authenticatedIdentityProvider.AuthenticatedUserId, Guid.Parse(getDmoWithDataDto.Id), cancellationToken);
        if (dmoWithData is null) {
            return NoContent();
        }

        var dmoWithDataDto = new DmoWithDataDto {
            Beats = dmoWithData.Beats.Select(_mapper.Map<BeatDto>).OrderBy(b => b.Order).ToList(),
            Characters = dmoWithData.Characters.Select(_mapper.Map<DmoCharacterDto>).ToList()
        };
        return OkWithData(dmoWithDataDto);
    }
    
    [HttpDelete]
    [Route("{DmoId}/tempIds")]
    public async Task<IActionResult> SanitizeTempIds([FromRoute] SanitizeTempIdsInDmoDto sanitizeTempIdsInDmoDto ) { // do not add cancellation token here
        var beats = await _dmosRepository.LoadBeatsWithCharactersAsync(
            _authenticatedIdentityProvider.AuthenticatedUserId, 
            Guid.Parse(sanitizeTempIdsInDmoDto.DmoId));

        foreach (var beat in beats) {
            SanitizeTempIdInBeatDescription(beat);
            if (beat.TempId != null) {
                beat.TempId = null;
            }
        }
        
        return NoContent();
    }


    private void SanitizeTempIdInBeatDescription(Beat beat) {
        if (string.IsNullOrWhiteSpace(beat.Description)) {
            return;
        }
        if (beat.Characters.Count == 0) {
            return;
        }

        var charactersWithTempIds = beat.Characters
            .Where(cha => cha.TempId != null)
            .ToList();

        if (charactersWithTempIds.Count == 0) {
            return;
        }

        var beatDesc = Uri.UnescapeDataString(beat.Description);
        if (string.IsNullOrWhiteSpace(beatDesc)) {
            return;
        }

        foreach (var characterWithTempIds in charactersWithTempIds) {
            beatDesc = beatDesc.Replace(characterWithTempIds.TempId!, characterWithTempIds.Id.ToString());
            characterWithTempIds.TempId = null;
        }

        beat.Description = Uri.EscapeDataString(beatDesc);
    }
}