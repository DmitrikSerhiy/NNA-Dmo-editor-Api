using API.DTO;
using AutoMapper;
using Model.Entities;

namespace API.Mappers {
    // ReSharper disable once UnusedMember.Global
    public class DmoMappers : Profile {

        public DmoMappers() {
            CreateMap<UserDmoCollection, DmoCollectionShortDto>().ReverseMap();
            CreateMap<Dmo, DmoShortDto>();
            CreateMap<UserDmoCollection, DmoCollectionDto>()
                .ForMember(udc => udc.DmoCount, dcd => dcd.MapFrom(dd => dd.Dmos.Count));
        }

    }
}
