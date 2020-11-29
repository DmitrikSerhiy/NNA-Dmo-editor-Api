using System.Text.Json;
using AutoMapper;
using Model.DTOs.Dmos;
using Model.Entities;

namespace API.Features.Dmos.Mappers {
    // ReSharper disable once UnusedMember.Global
    public class DmosMappers : Profile {
        public DmosMappers() {
            CreateMap<Dmo, ShortDmoWithBeatsDto>()
                .ForMember(udc => udc.Id, dcd => dcd.MapFrom(dd => dd.Id.ToString()))
                .ForMember(udc => udc.DmoStatus, dcd => dcd
                    .MapFrom(dd => StaticDmoMapper.GetDmoStatusString(dd.DmoStatus)))
                .ForMember(udc => udc.DmoStatusId, dcd => dcd.MapFrom(dd => dd.DmoStatus))
                .ForMember(udc => udc.Beats, dcd => dcd
                    .MapFrom(dd => DeserializeBeats(dd.BeatsJson)))
                .ReverseMap();

            CreateMap<Dmo, CreateDmoDto>()
                .ForMember(udc => udc.Id, dcd => dcd.MapFrom(dd => dd.Id.ToString()))
                .ReverseMap();
            CreateMap<Dmo, EditDmoInfoDto>()
                .ForMember(udc => udc.Id, dcd => dcd.MapFrom(dd => dd.Id.ToString()))
                .ReverseMap();

            CreateMap<Dmo, ShortDmoDto>()
                .ForMember(udc => udc.Id, dcd => dcd.MapFrom(dd => dd.Id.ToString()))
                .ReverseMap();
        }

        private BeatDto[] DeserializeBeats(string beatsJson) {
            return JsonSerializer.Deserialize<BeatDto[]>(beatsJson);
        }
    }
}
