using AutoMapper;
using NNA.Domain.DTOs.DmoCollections;
using NNA.Domain.DTOs.Dmos;
using NNA.Domain.Entities;
using NNA.Domain.Mappers;

namespace NNA.Api.Features.Dmos.Mappers;

public sealed class DmoMappers : Profile {
    public DmoMappers() {
        CreateMap<CreateDmoDto, Domain.DTOs.Editor.CreateDmoDto>().ReverseMap();
        CreateMap<Domain.DTOs.Editor.CreatedDmoDto, CreatedDmoDto>().ReverseMap();

        CreateMap<Dmo, DmoDetailsDto>()
            .ForMember(udc => udc.DmoStatus,
                dcd => dcd.MapFrom(dd => DmoStatusMapper.GetDmoStatusString(dd.DmoStatus)))
            .ForMember(udc => udc.DmoStatusId, dcd => dcd.MapFrom(dd => dd.DmoStatus));

        CreateMap<UpdateDmoDetailsDto, Dmo>();

    }
}
