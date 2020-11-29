using System.Linq;
using API.Features.Dmos.Mappers;
using AutoMapper;
using Model.DTOs.DmoCollections;
using Model.Entities;

namespace API.Features.DmoCollections.Mappers {
    // ReSharper disable once UnusedMember.Global
    public class DmoCollectionsMappers : Profile {

        public DmoCollectionsMappers() {
            CreateMap<DmoCollection, DmoCollectionShortDto>()
                .ForMember(udc => udc.DmoCount, dcd => dcd.MapFrom(dd => dd.DmoCollectionDmos.Count))
                .ReverseMap();
            CreateMap<Dmo, DmoShortDto>()
                .ForMember(udc => udc.DmoStatus,
                    dcd => dcd.MapFrom(dd => StaticDmoMapper.GetDmoStatusString(dd.DmoStatus)))
                .ForMember(udc => udc.DmoStatusId, dcd => dcd.MapFrom(dd => dd.DmoStatus))
                .ReverseMap();
            CreateMap<DmoCollection, DmoCollectionDto>()
                .ForMember(udc => udc.Dmos,
                    dcd => dcd.MapFrom(dd => dd.DmoCollectionDmos.Select(d => d.Dmo).ToArray()));
        }
    }
}
