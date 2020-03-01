using System;
using System.Linq;
using API.DTO;
using API.DTO.DmoCollections;
using AutoMapper;
using Model.Entities;
using Model.Enums;

namespace API.Mappers {
    // ReSharper disable once UnusedMember.Global
    public class DmoCollectionsMappers : Profile {

        public DmoCollectionsMappers() {
            CreateMap<UserDmoCollection, DmoCollectionShortDto>()
                .ForMember(udc => udc.DmoCount, dcd => dcd.MapFrom(dd => dd.DmoUserDmoCollections.Count))
                .ReverseMap();
            CreateMap<Dmo, DmoShortDto>()
                .ForMember(udc => udc.DmoStatus, dcd => dcd.MapFrom(dd => GetDmoStatusString(dd.DmoStatus)))
                .ForMember(udc => udc.DmoStatusId, dcd => dcd.MapFrom(dd => (Int16) dd.DmoStatus));
            CreateMap<UserDmoCollection, DmoCollectionDto>()
                .ForMember(udc => udc.Dmos,
                    dcd => dcd.MapFrom(dd => dd.DmoUserDmoCollections.Select(d => d.Dmo).ToArray()));
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
