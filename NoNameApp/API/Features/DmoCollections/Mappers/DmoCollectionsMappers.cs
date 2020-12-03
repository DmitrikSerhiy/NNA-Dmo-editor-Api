using System.Linq;
using AutoMapper;
using Model.DTOs.DmoCollections;
using Model.Entities;
using Model.Mappers;

namespace API.Features.DmoCollections.Mappers {
    // ReSharper disable once UnusedMember.Global
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
}
