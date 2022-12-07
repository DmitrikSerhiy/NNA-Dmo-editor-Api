﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using NNA.Api.Features.Characters.Services;
using NNA.Api.Features.Characters.Validators;
using NNA.Api.Features.Dmos.Validators;
using NNA.Domain.DTOs.Characters;
using NNA.Domain.DTOs.Dmos;
using NNA.Domain.Entities;
using NNA.Domain.Interfaces;
using NNA.Domain.Interfaces.Repositories;

namespace NNA.Api.Features.Characters.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public sealed class CharactersController : NnaController {
    private readonly IAuthenticatedIdentityProvider _authenticatedIdentityProvider;
    private readonly ICharactersRepository _charactersRepository;
    private readonly CharactersService _charactersService; 
    private readonly IDmosRepository _dmosRepository;
    private readonly IMapper _mapper;

    public CharactersController(
        ICharactersRepository repository, 
        IMapper mapper, 
        IDmosRepository dmosRepository, 
        IAuthenticatedIdentityProvider authenticatedIdentityProvider,
        CharactersService charactersService) {
        _charactersRepository = repository ?? throw new ArgumentNullException(nameof(repository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _dmosRepository = dmosRepository ?? throw new ArgumentNullException(nameof(dmosRepository));
        _authenticatedIdentityProvider = authenticatedIdentityProvider ?? throw new ArgumentNullException(nameof(authenticatedIdentityProvider));
        _charactersService = charactersService ?? throw new ArgumentNullException(nameof(charactersService));
    }
    
    [HttpGet]
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
            
            foreach (var beat in beats) {
                if (beat.TempId != null) {
                    beat.TempId = null;
                }
                if (string.IsNullOrWhiteSpace(beat.Description)) {
                    continue;
                }

                _charactersService.SanitizeCharactersTempIdsInBeatDescription(beat);
            }
        }

        var update = _mapper.Map(updateDto, characterToUpdate);
        _charactersRepository.UpdateCharacter(update);
        return NoContent();
    }
    
    [HttpDelete]
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

            foreach (var beat in beats) {
                if (beat.TempId != null) {
                    beat.TempId = null;
                }
                if (string.IsNullOrWhiteSpace(beat.Description)) {
                    continue;
                }

                _charactersService.SanitizeCharactersTempIdsInBeatDescription(beat);
                _charactersService.SanitizeRemovedCharactersInBeatDescription(beat, characterInBeatsIds);
            }
        }
        
        _charactersRepository.DeleteCharacter(characterToDelete);
        return NoContent();
    }
    
    [HttpPost("conflict/pair")]
    public IActionResult AddCharacterConflictPairToDmo(AddConflictPairToDmoDto conflictPairDto) {
        var antagonistInConflict = _mapper.Map<NnaMovieCharacterConflictInDmo>(conflictPairDto.Antagonist);
        var protagonistInConflict = _mapper.Map<NnaMovieCharacterConflictInDmo>(conflictPairDto.Protagonist);

        antagonistInConflict.PairOrder = conflictPairDto.Order;
        protagonistInConflict.PairOrder = conflictPairDto.Order;

        _charactersRepository.AddCharacterConflict(antagonistInConflict);
        _charactersRepository.AddCharacterConflict(protagonistInConflict);
        
        return NoContent();
    }
    
    [HttpPatch("conflict/pair")]
    public async Task<IActionResult> UpdateCharacterConflictPairToDmo([FromRoute] string id, [FromBody] JsonPatchDocument<UpdateCharacterConflictPairInDmo> patchDocument, CancellationToken cancellationToken) {
        var conflict = await _charactersRepository.LoadNnaCharacterConflictAsync(Guid.Parse(id), cancellationToken);
        if (conflict is null) {
            return NoContent();
        }

        var updateDto = _mapper.Map(conflict, new UpdateCharacterConflictPairInDmo());
        patchDocument.ApplyTo(updateDto);
        
        var validationResult = await new UpdateCharacterConflictPairInDmoValidator().ValidateAsync(updateDto, cancellationToken);
        if (!validationResult.IsValid) {
            return InvalidRequest(validationResult.Errors);
        }
        
        var update = _mapper.Map(updateDto, conflict);
        _charactersRepository.UpdateCharacterConflictInDmo(update);
        return NoContent();
    }
    
}