using System;
using AutoMapper;
using Model.DTOs.Beats;
using Model.DTOs.Editor;
using Model.Entities;
using Model.Mappers;

namespace API.Features.Beats.Mappers {
    public class BeatsMapper : Profile {
        public BeatsMapper() {
            CreateMap<Dmo, DmoWithBeatsJsonDto>()
                .ForMember(dmo => dmo.DmoId, dmoConfig => dmoConfig.MapFrom(dmoDto => dmoDto.Id.ToString()))
                .ForMember(dmo => dmo.DmoStatus, dmoConfig => dmoConfig.MapFrom(dmoDto => DmoStatusMapper.GetDmoStatusString(dmoDto.DmoStatus)))
                .ForMember(dmo => dmo.DmoStatusId, dmoConfig => dmoConfig.MapFrom(dmoDto => dmoDto.DmoStatus))
                .ForMember(dmo => dmo.BeatsJson, dmoConfig => dmoConfig.MapFrom(dmoDto => dmoDto.BeatsJson))
                .ReverseMap();

            CreateMap<Beat, BeatDto>()
                .ForMember(beat => beat.BeatId, beatConfig => beatConfig.MapFrom(beatDto => beatDto.Id))
                .ForMember(beat => beat.Text, beatConfig => beatConfig.MapFrom(beatDto => beatDto.Description))
                .ForMember(beat => beat.Time, beatConfig => beatConfig.MapFrom(beatDto => MapFromSeconds(beatDto.BeatTime)))
                .ReverseMap();
            
            CreateMap<Beat, CreateBeatDto>()
                .ForMember(beat => beat.DmoId, beatConfig => beatConfig.MapFrom(beatDto => beatDto.DmoId.ToString()))
                .ReverseMap();
        }
        
        private static BeatTimeDto MapFromSeconds(int seconds) {
            var timeSpan = TimeSpan.FromSeconds(seconds);
            return new BeatTimeDto {
                Hours = timeSpan.Hours,
                Minutes = timeSpan.Minutes,
                Seconds = timeSpan.Seconds
            };
        }
    }
}
