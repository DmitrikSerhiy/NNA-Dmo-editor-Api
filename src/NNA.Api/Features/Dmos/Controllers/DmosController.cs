using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using NNA.Api.Features.Characters.Services;
using NNA.Api.Features.Dmos.Validators;
using NNA.Api.Features.Editor.Validators;
using NNA.Domain.DTOs.Beats;
using NNA.Domain.DTOs.Characters;
using NNA.Domain.DTOs.Dmos;
using NNA.Domain.Entities;
using NNA.Domain.Enums;
using NNA.Domain.Interfaces;
using NNA.Domain.Interfaces.Repositories;
using UpdateDmoDetailsDtoValidator = NNA.Api.Features.Editor.Validators.UpdateDmoDetailsDtoValidator;

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
    public async Task<IActionResult> CreateDmo(CreateDmoDto newDmoDto) {
        var dmoDtoForEditor = _mapper.Map<Domain.DTOs.Editor.CreateDmoDto>(newDmoDto);
        var newDmo = await _editorService.CreateAndLoadDmo(dmoDtoForEditor, _authenticatedIdentityProvider.AuthenticatedUserId);
        return OkWithData(_mapper.Map<CreatedDmoDto>(newDmo));
    }

    [HttpGet("short/{id}")]
    public async Task<IActionResult> GetDmoDetailsShort([FromRoute] string id, CancellationToken cancellationToken) {
        var dmo = await _dmosRepository.GetShortById(Guid.Parse(id), cancellationToken);
        return dmo == null 
            ? NoContent() 
            : OkWithData(_mapper.Map<DmoDetailsShortDto>(dmo));
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDmoDetails([FromRoute] string id, CancellationToken cancellationToken) {
        var dmo = await _dmosRepository.GetByIdWithCharactersAndConflicts(Guid.Parse(id), cancellationToken);

        if (dmo == null) {
            return NoContent();
        }

        dmo.Conflicts = dmo.Conflicts
            .OrderByDescending(d => d.DateOfCreation)
            .ToList();
        
        var dmoDto = _mapper.Map<DmoDetailsDto>(dmo);

        return OkWithData(dmoDto);
    }

    [HttpPatch("{id}/details")]
    public async Task<IActionResult> UpdateDmoDetails([FromRoute] string id, [FromBody] JsonPatchDocument<UpdateDmoDetailsDto> patchDocument, CancellationToken cancellationToken) {
        var dmo = await _dmosRepository.GetById(Guid.Parse(id), cancellationToken, true);
        if (dmo is null) {
            return NoContent();
        }

        var updateDto = _mapper.Map(dmo, new UpdateDmoDetailsDto());
        patchDocument.ApplyTo(updateDto);
        
        var validationResult = await new UpdateDmoDetailsDtoValidator().ValidateAsync(updateDto, cancellationToken);
        if (!validationResult.IsValid) {
            return InvalidRequest(validationResult.Errors);
        }
        
        var update = _mapper.Map(updateDto, dmo);
        _dmosRepository.UpdateDmo(update);
        return NoContent();
    }
    
    [HttpPost("{id}/conflict")]
    public IActionResult CreateConflict([FromRoute] string id) {
        var dmoId = Guid.Parse(id);
        var pairId = Guid.NewGuid();
        var protagonistInConflict = new NnaMovieCharacterConflictInDmo {
            DmoId = dmoId,
            PairId = pairId,
            CharacterId = null,
            Achieved = false,
            CharacterType = (short)CharacterType.Protagonist
        };
        var antagonistInConflict = new NnaMovieCharacterConflictInDmo {
            DmoId = dmoId,
            PairId = pairId,
            CharacterId = null,
            Achieved = false,
            CharacterType = (short)CharacterType.Antagonist
        };
        
        _dmosRepository.CreateConflictInDmo(protagonistInConflict);
        _dmosRepository.CreateConflictInDmo(antagonistInConflict);
        return OkWithData(new { Protagonist = _mapper.Map<CreatedDmoConflictDto>(protagonistInConflict), Antagonist = _mapper.Map<CreatedDmoConflictDto>(antagonistInConflict) });
    }
    
    [HttpDelete("dmo/conflictPair/{conflictPairId}")]
    public async Task<IActionResult> DeleteDmoConflict([FromRoute] string conflictPairId, CancellationToken cancellationToken) {
        var conflicts = await _dmosRepository.GetNnaMovieCharacterConflictByPairId(Guid.Parse(conflictPairId), cancellationToken);
        if (conflicts.Count == 0) {
            return NoContent();
        }

        foreach (var conflict in conflicts) {
            _dmosRepository.DeleteConflictInDmo(conflict);
        }

        return NoContent();
    }
    
    [HttpPatch("dmo/conflict/{conflictId}")]
    public async Task<IActionResult> UpdateDmoConflict([FromRoute] string conflictId, [FromBody] JsonPatchDocument<UpdateDmoConflictDto> patchDocument, CancellationToken cancellationToken) {
        var conflict = await _dmosRepository.GetNnaMovieCharacterConflictById(Guid.Parse(conflictId), cancellationToken);
        if (conflict is null) {
            return NoContent();
        }
        
        var updateDto = _mapper.Map(conflict, new UpdateDmoConflictDto());
        patchDocument.ApplyTo(updateDto);
        
        var update = _mapper.Map(updateDto, conflict);
        _dmosRepository.UpdateConflictInDmo(update);
        return NoContent();
    }
    
    [HttpPatch("{id}/plot")]
    public async Task<IActionResult> UpdateDmoPlotDetails([FromRoute] string id, [FromBody] JsonPatchDocument<UpdateDmoPlotDetailsDto> patchDocument, CancellationToken cancellationToken) {
        var dmo = await _dmosRepository.GetById(Guid.Parse(id), cancellationToken, true);
        if (dmo is null) {
            return NoContent();
        }

        var updateDto = _mapper.Map(dmo, new UpdateDmoPlotDetailsDto());
        patchDocument.ApplyTo(updateDto);
        
        var validationResult = await new UpdateDmoPlotDetailsDtoValidator().ValidateAsync(updateDto, cancellationToken);
        if (!validationResult.IsValid) {
            return InvalidRequest(validationResult.Errors);
        }
        
        var update = _mapper.Map(updateDto, dmo);
        _dmosRepository.UpdateDmo(update);
        return NoContent();
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