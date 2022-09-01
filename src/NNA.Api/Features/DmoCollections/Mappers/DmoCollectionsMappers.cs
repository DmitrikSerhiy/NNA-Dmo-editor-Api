using AutoMapper;
using NNA.Domain.DTOs.DmoCollections;
using NNA.Domain.Entities;
using NNA.Domain.Mappers;

namespace NNA.Api.Features.DmoCollections.Mappers;

public class DmoCollectionsMappers : Profile {
    public DmoCollectionsMappers() {
        CreateMap<DmoCollection, DmoCollectionShortDto>()
            .ForMember(udc => udc.DmoCount, dcd => dcd.MapFrom(dd => dd.DmoCollectionDmos.Count))
            .ReverseMap();
        CreateMap<Dmo, DmoShortDto>()
            .ForMember(udc => udc.DmoStatus,
                dcd => dcd.MapFrom(dd => DmoStatusMapper.GetDmoStatusString(dd.DmoStatus)))
            .ForMember(udc => udc.DmoStatusId, dcd => dcd.MapFrom(dd => dd.DmoStatus))
            .ReverseMap();
        CreateMap<DmoCollection, DmoCollectionDto>()
            .ForMember(udc => udc.Dmos,
                dcd => dcd.MapFrom(dd => dd.DmoCollectionDmos.Select(d => d.Dmo).ToArray()));
    }
}