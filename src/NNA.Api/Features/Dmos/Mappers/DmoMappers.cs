using AutoMapper;
using NNA.Domain.DTOs.Dmos;
using NNA.Domain.DTOs.Editor;

namespace NNA.Api.Features.Dmos.Mappers;

public class DmoMappers : Profile {
    public DmoMappers() {
        CreateMap<CreateDmoByHttpDto, CreateDmoDto>().ReverseMap();
        CreateMap<CreatedDmoDto, CreatedDmoByHttpDto>().ReverseMap();
    }
}
