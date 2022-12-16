using AutoMapper;
using NNA.Domain.DTOs.Beats;
using NNA.Domain.DTOs.Tags;
using NNA.Domain.Entities;

namespace NNA.Api.Features.Tags.Mappers;

public sealed class TagsMapper : Profile {
    public TagsMapper() {
        CreateMap<NnaTag, TagWithoutDescriptionDto>();
        CreateMap<NnaTag, TagDto>();
        CreateMap<NnaTagInBeat, NnaTagInBeatDto>()
            .ForMember(t => t.Id, entity => entity.MapFrom(tig => tig.Id))
            .ForMember(t => t.Name, entity => entity.MapFrom(tig => tig.Tag.Name));
    }
}