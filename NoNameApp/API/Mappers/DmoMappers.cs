using System;
using API.DTO;
using AutoMapper;
using Model.Entities;
using Model.Enums;

namespace API.Mappers {
    // ReSharper disable once UnusedMember.Global
    public class DmoMappers : Profile {

        public DmoMappers() {
            CreateMap<UserDmoCollection, DmoCollectionShortDto>().ReverseMap();
            CreateMap<Dmo, DmoShortDto>()
                .ForMember(udc => udc.DmoStatus, dcd => dcd.MapFrom(dd => GetDmoStatusString(dd.DmoStatus)))
                .ForMember(udc => udc.DmoStatusId, dcd => dcd.MapFrom(dd => (Int16) dd.DmoStatus));
            CreateMap<UserDmoCollection, DmoCollectionDto>();
            //.ForMember(udc => udc.DmoCount, dcd => dcd.MapFrom(dd => dd.Dmos.Count));
        }

        private String GetDmoStatusString(Int16 status) {
            switch (status) {
                case (Int16)DmoStatus.Complete:
                    return nameof(DmoStatus.Complete);
                case (Int16)DmoStatus.InProgress:
                    return nameof(DmoStatus.InProgress);
                case (Int16)DmoStatus.New:
                    return nameof(DmoStatus.New);
                case (Int16)DmoStatus.NotFinished:
                    return nameof(DmoStatus.NotFinished);
                default:
                    return nameof(DmoStatus.New);
            }
        }

    }
}
