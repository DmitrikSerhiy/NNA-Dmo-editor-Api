using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using NNA.Api.Attributes;
using NNA.Api.Features.Characters.Services;
using NNA.Api.Features.Characters.Validators;
using NNA.Domain.DTOs.Characters;
using NNA.Domain.Entities;
using NNA.Domain.Interfaces;
using NNA.Domain.Interfaces.Repositories;

namespace NNA.Api.Features.Characters.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class CharactersController : NnaController {
    private readonly IAuthenticatedIdentityProvider _authenticatedIdentityProvider;
    private readonly ICharactersRepository _charactersRepository;
    private readonly TempIdSanitizer _tempIdSanitizer; 
    private readonly IDmosRepository _dmosRepository;
    private readonly IMapper _mapper;

    public CharactersController(
        ICharactersRepository repository, 
        IMapper mapper, 
        IDmosRepository dmosRepository, 
        IAuthenticatedIdentityProvider authenticatedIdentityProvider,
        TempIdSanitizer tempIdSanitizer) {
        _charactersRepository = repository ?? throw new ArgumentNullException(nameof(repository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _dmosRepository = dmosRepository ?? throw new ArgumentNullException(nameof(dmosRepository));
        _authenticatedIdentityProvider = authenticatedIdentityProvider ?? throw new ArgumentNullException(nameof(authenticatedIdentityProvider));
        _tempIdSanitizer = tempIdSanitizer ?? throw new ArgumentNullException(nameof(tempIdSanitizer));
    }
    
    [HttpGet]
    [NotActiveUserAuthorize]
    public async Task<IActionResult> GetDmoCharacters([FromQuery] GetCharactersDto charactersDto, CancellationToken cancellationToken) {
        var characters = await _charactersRepository.GetDmoCharactersWithBeatsAsync(charactersDto.DmoId, cancellationToken);
        var charactersWithBeatsDto = characters
            .Select(character => {
                var characterWithBeat = _mapper.Map<DmoCharacterDto>(character);
                if (character.Beats.Count > 0) {
                    characterWithBeat.CharacterBeatIds = character.Beats.Select(cha => cha.BeatId).ToArray();
                }
                return characterWithBeat;
            })
            .OrderByDescending(cha => cha.Count)
            .ToList();

        return OkWithData(charactersWithBeatsDto);
    }
 
    [HttpPost]
    [ActiveUserAuthorize]
    public async Task<IActionResult> CreateCharacter([FromBody] CreateCharacterDto createCharacterDto, CancellationToken cancellationToken) {
        var isCharacterNameTaken =
            await _charactersRepository.IsExistAsync(createCharacterDto.Name, createCharacterDto.DmoId, cancellationToken);

        if (isCharacterNameTaken) {
            return BadRequestWithMessageToToastr("Character name is already taken");
        }
        
        var characterEntity = _mapper.Map<NnaMovieCharacter>(createCharacterDto);
        _charactersRepository.CreateCharacter(characterEntity);
        return NoContent();
    }
    
    [HttpPatch("{id}")]
    [ActiveUserAuthorize]
    public async Task<IActionResult> UpdateCharacter([FromRoute] string id, [FromBody] JsonPatchDocument<UpdateCharacterDto> patchDocument, CancellationToken cancellationToken) {
        var characterToUpdate = await _charactersRepository.GetCharacterByIdAsync(Guid.Parse(id), cancellationToken);
        if (characterToUpdate is null) {
            return NoContent();
        }
        
        var updateDto = _mapper.Map(characterToUpdate, new UpdateCharacterDto());
        patchDocument.ApplyTo(updateDto);
        
        var validationResult = await new UpdateCharacterDtoValidator().ValidateAsync(updateDto, cancellationToken);
        if (!validationResult.IsValid) {
            return InvalidRequest(validationResult.Errors);
        }
        
        if (characterToUpdate.Name != updateDto.Name) {
            var isCharacterNameTaken =
                await _charactersRepository.IsExistAsync(updateDto.Name, updateDto.DmoId, cancellationToken);
            
            if (isCharacterNameTaken) {
                return BadRequestWithMessageToToastr("Character name is already taken");
            }
        }
        
        var characterInBeatsIds = await _charactersRepository.LoadCharacterInBeatIdsAsync(characterToUpdate.Id);
        if (characterInBeatsIds.Count > 0) {
            var beats = await _dmosRepository.LoadBeatsWithCharactersAsync(
                _authenticatedIdentityProvider.AuthenticatedUserId, 
                characterToUpdate.DmoId);

            for (var i = 0; i < beats.Count; i++) {
                if (beats[i].TempId != null) {
                    beats[i].TempId = null;
                }

                beats[i].Order = i;
                if (string.IsNullOrWhiteSpace(beats[i].Description)) {
                    continue;
                }

                _tempIdSanitizer.SanitizeCharactersTempIdsInBeatDescription(beats[i]);
                _tempIdSanitizer.SanitizeTagsTempIdsInBeatDescription(beats[i]);
            }
        }

        var update = _mapper.Map(updateDto, characterToUpdate);
        _charactersRepository.UpdateCharacter(update);
        return NoContent();
    }
    
    [HttpDelete]
    [ActiveUserAuthorize]
    public async Task<IActionResult> DeleteCharacter([FromQuery] DeleteCharacterDto deleteCharacterDto, CancellationToken cancellationToken) {
        var characterToDelete = await _charactersRepository.GetCharacterByIdAsync(deleteCharacterDto.Id, cancellationToken);
        if (characterToDelete is null) {
            return NoContent();
        }
        
        var characterInBeatsIds = await _charactersRepository.LoadCharacterInBeatIdsAsync(deleteCharacterDto.Id);
        if (characterInBeatsIds.Count > 0) {
            var beats = await _dmosRepository.LoadBeatsWithCharactersAsync(
                _authenticatedIdentityProvider.AuthenticatedUserId, 
                characterToDelete.DmoId);

            for (var i = 0; i < beats.Count; i++) {
                if (beats[i].TempId != null) {
                    beats[i].TempId = null;
                }

                beats[i].Order = i;
                _tempIdSanitizer.SanitizeTagsTempIdsInBeatDescription(beats[i]);
                _tempIdSanitizer.SanitizeCharactersTempIdsInBeatDescription(beats[i]);
                _tempIdSanitizer.SanitizeRemovedCharactersInBeatDescription(beats[i], characterInBeatsIds);
            }
        }
        
        _charactersRepository.DeleteCharacter(characterToDelete);
        return NoContent();
    }
}
