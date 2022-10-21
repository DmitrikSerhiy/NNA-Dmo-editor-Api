using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NNA.Domain.DTOs.Characters;
using NNA.Domain.Entities;
using NNA.Domain.Interfaces.Repositories;

namespace NNA.Api.Features.Characters.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public sealed class CharactersController : NnaController {
    private readonly ICharactersRepository _repository;
    private readonly IMapper _mapper;

    public CharactersController(ICharactersRepository repository, IMapper mapper) {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
    
    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetDmoCharacters([FromQuery] GetCharactersDto charactersDto, CancellationToken cancellationToken) {
        var characters = await _repository.GetDmoCharactersAsync(Guid.Parse(charactersDto.DmoId), cancellationToken);
        return OkWithData(characters.Select(_mapper.Map<DmoCharacterDto>));
    }

    [HttpPost]
    [Route("")]
    public async Task<IActionResult> CreateCharacter([FromBody] CreateCharacterDto createCharacterDto, CancellationToken cancellationToken) {
        var isCharacterNameTaken =
            await _repository.IsExist(createCharacterDto.Name, Guid.Parse(createCharacterDto.DmoId), cancellationToken);

        if (isCharacterNameTaken) {
            return BadRequestWithMessageForUi("Character name is already taken");
        }
        
        var characterEntity = _mapper.Map<NnaMovieCharacter>(createCharacterDto);
        _repository.CreateCharacter(characterEntity);
        return NoContent();
    }
    
    [HttpPut]
    [Route("")]
    public async Task<IActionResult> UpdateCharacter([FromBody] UpdateCharacterDto updateCharacterDto, CancellationToken cancellationToken) {
        var isCharacterNameTaken =
            await _repository.IsExist(updateCharacterDto.Name, Guid.Parse(updateCharacterDto.DmoId), cancellationToken);

        if (isCharacterNameTaken) {
            return BadRequestWithMessageForUi("Character name is already taken");
        }
        
        var characterToUpdate = await _repository.GetCharacterById(Guid.Parse(updateCharacterDto.Id), cancellationToken);
        if (characterToUpdate is null) {
            return NoContent();
        }

        _repository.UpdateCharactersNameAndAliases(characterToUpdate, updateCharacterDto.Name, updateCharacterDto.Aliases);
        return NoContent();
    }
    
    [HttpDelete]
    [Route("")]
    public async Task<IActionResult> DeleteCharacter([FromQuery] DeleteCharacterDto deleteCharacterDto, CancellationToken cancellationToken) {
        var characterToDelete = await _repository.GetCharacterById(Guid.Parse(deleteCharacterDto.Id), cancellationToken);
        if (characterToDelete is null) {
            return NoContent();
        }

        _repository.DeleteCharacter(characterToDelete);
        return NoContent();
    }

}
