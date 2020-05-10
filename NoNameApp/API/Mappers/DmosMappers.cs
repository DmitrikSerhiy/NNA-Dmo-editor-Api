using API.DTO.Dmos;
using API.Helpers;
using AutoMapper;
using Model.Entities;
using System;
using System.Text.Json;

namespace API.Mappers {
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

            CreateMap<Beat, BeatDto>()
                .ForMember(udc => udc.PlotTimeSpot, dcd => dcd
                    .MapFrom(dd => new TimeSpan(0, dd.PlotTimeSpot.Hours, dd.PlotTimeSpot.Minutes)))
                .ReverseMap();
        }

        private BeatDto[] DeserializeBeats(String beatsJson) {
            return JsonSerializer.Deserialize<BeatDto[]>(beatsJson);
        }
    }
}
