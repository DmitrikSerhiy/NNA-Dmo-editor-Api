using System.Text.Json;
using AutoMapper;
using Model.DTOs.Dmos;
using Model.Entities;
using Model.Mappers;

namespace API.Features.Dmos.Mappers {
    // ReSharper disable once UnusedMember.Global
    public class DmosMappers : Profile {
        public DmosMappers() {
            CreateMap<Dmo, ShortDmoWithBeatsDto>()
                .ForMember(udc => udc.Id, dcd => dcd.MapFrom(dd => dd.Id.ToString()))
                .ForMember(udc => udc.DmoStatus, dcd => dcd
                    .MapFrom(dd => DmoStatusMapper.GetDmoStatusString(dd.DmoStatus)))
                .ForMember(udc => udc.DmoStatusId, dcd => dcd.MapFrom(dd => dd.DmoStatus))
                .ForMember(udc => udc.Beats, dcd => dcd
                    .MapFrom(dd => DeserializeBeats(dd.BeatsJson)))
                .ReverseMap();
        }

        private BeatDto[] DeserializeBeats(string beatsJson) {
            return JsonSerializer.Deserialize<BeatDto[]>(beatsJson);
        }
    }
}
