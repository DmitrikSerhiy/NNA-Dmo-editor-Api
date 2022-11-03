using AutoMapper;
using NNA.Domain.DTOs.Beats;
using NNA.Domain.DTOs.Characters;
using NNA.Domain.Entities;

namespace NNA.Api.Features.Characters.Mappers;

public sealed class CharacterMapper : Profile {
    public CharacterMapper() {
        CreateMap<NnaMovieCharacter, DmoCharacterDto>()
            .ForMember(dto => dto.Count, entity => entity.MapFrom(chaEn => chaEn.Beats.Count));
        CreateMap<CreateCharacterDto, NnaMovieCharacter>();
        CreateMap<NnaMovieCharacterInBeat, NnaMovieCharacterInBeatDto>()
            .ForMember(dto => dto.CharacterId, entity => entity.MapFrom(chaEn => chaEn.Character.Id))
            .ForMember(dto => dto.Name, entity => entity.MapFrom(chaEn => chaEn.Character.Name));
    }
}
