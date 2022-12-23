using AutoMapper;
using NNA.Domain.DTOs.Community;
using NNA.Domain.Entities;

namespace NNA.Api.Features.Community.Mappers; 

public sealed class CommunityMappers : Profile {
    public CommunityMappers() {
        CreateMap<Dmo, PublishedDmoShortDto>()
            .ForMember(d => d.DmoStatusId, dcd => dcd.MapFrom(dd => dd.DmoStatus))
            .ForMember(d => d.AuthorNickname, dcd => dcd.MapFrom(dd => dd.NnaUser.UserName))
            .ForMember(d => d.PublishDate, dcd => dcd.MapFrom(dd =>  new DateTimeOffset(dd.PublishDate!.Value, TimeSpan.Zero).ToString("g") ));

    }
}