using AutoMapper;
using NNA.Domain.DTOs.Editor;
using NNA.Domain.Entities;

namespace NNA.Api.Features.Editor.Mappers;
public class DmoMapper : Profile {
    public DmoMapper() {
        CreateMap<CreateDmoDto, Dmo>();
        CreateMap<Dmo, CreatedDmoDto>();

        CreateMap<UpdateShortDmoDto, Dmo>();

        CreateMap<LoadShortDmoDto, Dmo>();
        CreateMap<Dmo, LoadedShortDmoDto>();
    }
}

