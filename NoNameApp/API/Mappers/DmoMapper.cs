using API.DTO;
using AutoMapper;
using Model.Entities;

namespace API.Mappers {
    // ReSharper disable once UnusedMember.Global
    public class DmoMapper : Profile {

        public DmoMapper() {
            CreateMap<DmoDto, Dmo>();
            CreateMap<UserDmoCollection, DmoListDto>().ReverseMap();
        }

    }
}
