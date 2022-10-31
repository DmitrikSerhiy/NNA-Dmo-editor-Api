using AutoMapper;
using NNA.Domain.DTOs.Beats;
using NNA.Domain.DTOs.Characters;
using NNA.Domain.Entities;

namespace NNA.Api.Features.Characters.Mappers;

public sealed class CharacterMapper : Profile {
    public CharacterMapper() {
        CreateMap<NnaMovieCharacter, DmoCharacterDto>();
        CreateMap<CreateCharacterDto, NnaMovieCharacter>();
        CreateMap<NnaMovieCharacter, NnaMovieCharacterInBeatDto>();
    }
}
