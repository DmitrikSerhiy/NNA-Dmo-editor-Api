using AutoMapper;
using NNA.Domain.DTOs.Dmos;
using NNA.Domain.Entities;

namespace NNA.Api.Features.Dmos.Mappers;

public sealed class DmoMappers : Profile {
    public DmoMappers() {
        CreateMap<CreateDmoDto, Domain.DTOs.Editor.CreateDmoDto>().ReverseMap();
        CreateMap<Domain.DTOs.Editor.CreatedDmoDto, CreatedDmoDto>().ReverseMap();

        CreateMap<NnaMovieCharacterConflictInDmo, DmoConflictDto>();
        CreateMap<NnaMovieCharacterConflictInDmo, CreatedDmoConflictDto>();
        CreateMap<NnaMovieCharacterConflictInDmo, UpdateDmoConflictDto>().ReverseMap();

        CreateMap<NnaMovieCharacter, DmoCharactersForConflictDto>()
            .ForMember(udc => udc.CharacterId, dcd => dcd.MapFrom(dd => dd.Id));

        CreateMap<Dmo, DmoDetailsDto>()
            .ForMember(udc => udc.DmoStatusId, dcd => dcd.MapFrom(dd => dd.DmoStatus))
            .ForMember(udc => udc.ControllingIdeaId, dcd => dcd.MapFrom(dd => dd.ControllingIdeaId))
            .ForMember(udc => udc.CharactersForConflict, dcd => dcd.MapFrom(dd => dd.Characters))
            .ForMember(udc => udc.Conflicts, dcd => dcd.MapFrom(dd => dd.Conflicts));

        CreateMap<Dmo, DmoDetailsShortDto>()
            .ForMember(udc => udc.DmoStatusId, dcd => dcd.MapFrom(dd => dd.DmoStatus));
        
        CreateMap<PatchDmoDetailsDto, Dmo>()
            .ForMember(udc => udc.DmoStatus, dcd => dcd.MapFrom(dd => dd.DmoStatusId))
            .ReverseMap();
        
        CreateMap<UpdateDmoPlotDetailsDto, Dmo>()
            .ForMember(udc => udc.ControllingIdeaId, dcd => dcd.MapFrom(dd => dd.ControllingIdeaId))
            .ReverseMap();
        
        
    }
}
