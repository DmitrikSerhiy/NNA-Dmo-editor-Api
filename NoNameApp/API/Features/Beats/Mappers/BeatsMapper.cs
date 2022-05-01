using AutoMapper;
using Model.DTOs.Beats;
using Model.DTOs.Editor;
using Model.Entities;
using Model.Mappers;

namespace API.Features.Beats.Mappers
{
    public class BeatsMapper : Profile
    {
        public BeatsMapper() {
            CreateMap<Dmo, DmoWithBeatsJsonDto>()
                .ForMember(udc => udc.DmoId, dcd => dcd.MapFrom(dd => dd.Id.ToString()))
                .ForMember(udc => udc.DmoStatus, dcd => dcd
                    .MapFrom(dd => DmoStatusMapper.GetDmoStatusString(dd.DmoStatus)))
                .ForMember(udc => udc.DmoStatusId, dcd => dcd.MapFrom(dd => dd.DmoStatus))
                .ForMember(udc => udc.BeatsJson, dcd => dcd
                    .MapFrom(dd => dd.BeatsJson))
                .ReverseMap();

            CreateMap<Beat, CreateBeatDto>()
                .ForMember(b => b.DmoId, bb => bb.MapFrom(bbb => bbb.DmoId.ToString()))
                .ReverseMap();

        }
    }
}
