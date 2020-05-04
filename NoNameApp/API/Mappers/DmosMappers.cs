using API.DTO.Dmos;
using API.Helpers;
using AutoMapper;
using Model.Entities;
using System;
using System.Linq;

namespace API.Mappers {
    // ReSharper disable once UnusedMember.Global
    public class DmosMappers : Profile {
        public DmosMappers() {
            CreateMap<Dmo, DmoDto>()
                .ForMember(udc => udc.DmoStatus, dcd => dcd
                    .MapFrom(dd => StaticDmoMapper.GetDmoStatusString(dd.DmoStatus)))
                .ForMember(udc => udc.DmoStatusId, dcd => dcd.MapFrom(dd => dd.DmoStatus))
                .ReverseMap();

            CreateMap<BeatDto, Beat>()
                .ForMember(udc => udc.PlotTimeSpot, dcd => dcd
                    .MapFrom(dd => new TimeSpan(0, dd.PlotTimeSpot.Hours, dd.PlotTimeSpot.Minutes)))
                .ReverseMap();

            CreateMap<Dmo, PartialDmoWithBeatsDto>()
                .ForMember(udc => udc.DmoId, dcd => dcd.MapFrom(dd => dd.Id))
                .ForMember(udc => udc.Beats, dcd => dcd.MapFrom(dd => dd.Beats.ToArray()))
                .ReverseMap();
        }
    }
}
