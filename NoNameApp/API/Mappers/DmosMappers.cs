using API.DTO.Dmos;
using API.Helpers;
using AutoMapper;
using Model.Entities;
using System;

namespace API.Mappers {
    // ReSharper disable once UnusedMember.Global
    public class DmosMappers : Profile {
        public DmosMappers() {
            CreateMap<Dmo, DmoDto>()
                .ForMember(udc => udc.DmoStatus, dcd => dcd.MapFrom(dd => StaticDmoMapper.GetDmoStatusString(dd.DmoStatus)))
                .ForMember(udc => udc.DmoStatusId, dcd => dcd.MapFrom(dd => dd.DmoStatus));
        }
    }
}
