using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NNA.Api.Features.Characters.Services;
using NNA.Domain.DTOs.Beats;
using NNA.Domain.DTOs.Characters;
using NNA.Domain.DTOs.DmoCollections;
using NNA.Domain.DTOs.Dmos;
using NNA.Domain.DTOs.Editor;
using NNA.Domain.Interfaces;
using NNA.Domain.Interfaces.Repositories;

namespace NNA.Api.Features.Dmos.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public sealed class DmosController : NnaController {
    private readonly IMapper _mapper;
    private readonly CharactersService _charactersService;
    private readonly IAuthenticatedIdentityProvider _authenticatedIdentityProvider;
    private readonly IDmosRepository _dmosRepository;
    private readonly IEditorService _editorService;

    public DmosController(
        IMapper mapper,
        IDmosRepository dmosRepository,
        IEditorService editorService,
        IAuthenticatedIdentityProvider authenticatedIdentityProvider, 
        CharactersService charactersService) {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _dmosRepository = dmosRepository ?? throw new ArgumentNullException(nameof(dmosRepository));
        _editorService = editorService ?? throw new ArgumentNullException(nameof(editorService));
        _authenticatedIdentityProvider = authenticatedIdentityProvider ?? throw new ArgumentNullException(nameof(authenticatedIdentityProvider));
        _charactersService = charactersService ?? throw new ArgumentNullException(nameof(charactersService));
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
    [Route("{Id}/withData")]
    public async Task<IActionResult> LoadDmoWithData([FromRoute] GetDmoWithDataDto getDmoWithDataDto, CancellationToken cancellationToken, [FromQuery] bool sanitizeBeforeLoad = false) {
        if (sanitizeBeforeLoad) {
            await SanitizeTempIds(new SanitizeTempIdsInDmoDto { DmoId = getDmoWithDataDto.Id} );
            await _dmosRepository.SyncContextImmediatelyAsync(cancellationToken);
        }
        
        var dmoWithData = await _dmosRepository.GetDmoWithDataAsync(_authenticatedIdentityProvider.AuthenticatedUserId, getDmoWithDataDto.Id, cancellationToken);
        if (dmoWithData is null) {
            return NoContent();
        }

        var dmoWithDataDto = new DmoWithDataDto {
            Beats = dmoWithData.Beats.Select(_mapper.Map<BeatDto>).ToList(),
            Characters = dmoWithData.Characters.Select(_mapper.Map<DmoCharacterDto>).ToList()
        };

        var allCharacters = new List<NnaMovieCharacterInBeatDto>();
        allCharacters.AddRange(dmoWithDataDto.Beats.SelectMany(beat => beat.CharactersInBeat).ToList());
        var groupedCharacters = allCharacters.GroupBy(cha => cha.CharacterId).ToList();
        
        foreach (var characterInDmo in dmoWithDataDto.Characters) {
            var group = groupedCharacters
                .FirstOrDefault(gCha => gCha.Key.ToString() == characterInDmo.Id);
            characterInDmo.Count = group?.Count() ?? 0;
        }

        foreach (var beat in dmoWithDataDto.Beats) {
            foreach (var characterInBeat in beat.CharactersInBeat) {
                characterInBeat.Color = dmoWithDataDto.Characters
                    .FirstOrDefault(cha => cha.Id == characterInBeat.CharacterId.ToString())?.Color ?? "#000000";
            }
        }
        dmoWithDataDto.Characters = dmoWithDataDto.Characters.OrderByDescending(cha => cha.Count).ToList();
        dmoWithDataDto.Beats = dmoWithDataDto.Beats.OrderBy(b => b.Order).ToList();
        return OkWithData(dmoWithDataDto);
    }
    
    [HttpDelete]
    [Route("{DmoId}/tempIds")]
    public async Task<IActionResult> SanitizeTempIds([FromRoute] SanitizeTempIdsInDmoDto sanitizeTempIdsInDmoDto ) { // do not add cancellation token here
        var beats = await _dmosRepository.LoadBeatsWithCharactersAsync(
            _authenticatedIdentityProvider.AuthenticatedUserId, 
            sanitizeTempIdsInDmoDto.DmoId);

        foreach (var beat in beats) {
            if (beat.TempId != null) {
                beat.TempId = null;
            }
            _charactersService.SanitizeCharactersTempIdsInBeatDescription(beat);
        }
        
        return NoContent();
    }
}