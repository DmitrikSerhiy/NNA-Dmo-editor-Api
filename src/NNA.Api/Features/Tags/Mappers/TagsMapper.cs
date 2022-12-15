using AutoMapper;
using NNA.Domain.DTOs.Tags;
using NNA.Domain.Entities;

namespace NNA.Api.Features.Tags.Mappers;

public sealed class TagsMapper : Profile {
    public TagsMapper() {
        CreateMap<NnaTag, TagWithoutDescriptionDto>();
        CreateMap<NnaTag, TagDto>();
    }
}