using AutoMapper;
using NNA.Domain.DTOs.DmoCollections;
using NNA.Domain.DTOs.Dmos;
using NNA.Domain.Entities;

namespace NNA.Api.Features.Dmos.Mappers;

public sealed class DmoMappers : Profile {
    public DmoMappers() {
        CreateMap<CreateDmoDto, Domain.DTOs.Editor.CreateDmoDto>().ReverseMap();
        CreateMap<Domain.DTOs.Editor.CreatedDmoDto, CreatedDmoDto>().ReverseMap();

        CreateMap<Dmo, DmoDetailsDto>()
            .ForMember(udc => udc.DmoStatusId, dcd => dcd.MapFrom(dd => dd.DmoStatus))
            .ForMember(udc => udc.ControllingIdeaId, dcd => dcd.MapFrom(dd => dd.ControllingIdeaId));


        CreateMap<Dmo, DmoDetailsShortDto>()
            .ForMember(udc => udc.DmoStatusId, dcd => dcd.MapFrom(dd => dd.DmoStatus));
        
        CreateMap<UpdateDmoDetailsDto, Dmo>()
            .ForMember(udc => udc.DmoStatus, dcd => dcd.MapFrom(dd => dd.DmoStatusId))
            .ReverseMap();
        
        CreateMap<UpdateDmoPlotDetailsDto, Dmo>()
            .ForMember(udc => udc.ControllingIdeaId, dcd => dcd.MapFrom(dd => dd.ControllingIdeaId))
            .ReverseMap();
        
        
    }
}
