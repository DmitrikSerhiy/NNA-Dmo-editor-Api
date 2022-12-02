using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using NNA.Api.Features.Characters.Services;
using NNA.Api.Features.Characters.Validators;
using NNA.Domain.DTOs.Characters;
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
        //todo: load character
        return OkWithData(characters.Select(_mapper.Map<DmoCharacterDto>).OrderByDescending(cha => cha.Count));
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
}
