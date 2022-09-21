using AutoMapper;
using NNA.Domain.DTOs.Beats;
using NNA.Domain.DTOs.Editor;
using NNA.Domain.Entities;
using NNA.Domain.Mappers;

namespace NNA.Api.Features.Beats.Mappers;

public class BeatsMapper : Profile {
    private static string DefaultTimeView { get; } = "0:00:00";

    public BeatsMapper() {
        CreateMap<Dmo, DmoWithBeatsJsonDto>()
            .ForMember(dmo => dmo.DmoId, dmoConfig => dmoConfig.MapFrom(dmoDto => dmoDto.Id.ToString()))
            .ForMember(dmo => dmo.DmoStatus,
                dmoConfig => dmoConfig.MapFrom(dmoDto => DmoStatusMapper.GetDmoStatusString(dmoDto.DmoStatus)))
            .ForMember(dmo => dmo.DmoStatusId, dmoConfig => dmoConfig.MapFrom(dmoDto => dmoDto.DmoStatus))
            .ForMember(dmo => dmo.BeatsJson, dmoConfig => dmoConfig.MapFrom(dmoDto => dmoDto.BeatsJson))
            .ReverseMap();

        CreateMap<Beat, BeatDto>()
            .ForMember(beat => beat.BeatId, beatConfig => beatConfig.MapFrom(beatDto => beatDto.Id))
            .ForMember(beat => beat.Text, beatConfig => beatConfig.MapFrom(beatDto => beatDto.Description))
            .ForMember(beat => beat.Time, beatConfig => beatConfig.MapFrom(beatDto => MapFromSeconds(beatDto.BeatTime)))
            .ReverseMap();

        CreateMap<CreateBeatDto, Beat>()
            .ForMember(beat => beat.DmoId, beatConfig => beatConfig.MapFrom(beatDto => beatDto.DmoId!.ToString()))
            .ForMember(beat => beat.BeatTimeView, beatConfig => beatConfig.MapFrom(beatDto => DefaultTimeView))
            .ForMember(beat => beat.BeatTime, beatConfig => beatConfig.MapFrom(beatDto => 0))
            .ForMember(beat => beat.Description, beatConfig => beatConfig.MapFrom(beatDto => string.Empty));

        CreateMap<UpdateBeatDto, Beat>()
            .ForMember(beat => beat.Description, beatConfig => beatConfig.MapFrom(beatDto => beatDto.Text))
            .ForMember(beat => beat.BeatTime, beatConfig => beatConfig.MapFrom(beatDto => MapToSeconds(beatDto.Time)))
            .ForMember(beat => beat.BeatTimeView,
                beatConfig => beatConfig.MapFrom(beatDto => MapToTimeView(beatDto.Time)));

        CreateMap<RemoveBeatDto, Beat>();
        CreateMap<BeatToSwapDto, Beat>();
    }

    private static BeatTimeDto MapFromSeconds(int seconds) {
        var timeSpan = TimeSpan.FromSeconds(seconds);
        return new BeatTimeDto {
            Hours = timeSpan.Hours,
            Minutes = timeSpan.Minutes,
            Seconds = timeSpan.Seconds
        };
    }

    private static int MapToSeconds(UpdateBeatTimeDto timeDto) {
        return timeDto.Seconds + (60 * timeDto.Minutes) + (60 * 60 * timeDto.Hours);
    }

    private static string MapToTimeView(UpdateBeatTimeDto timeDto) {
        var timeSpanResult = TimeSpan.FromSeconds(MapToSeconds(timeDto));
        return timeSpanResult.ToString("g").Substring(0, 7);
    }
}