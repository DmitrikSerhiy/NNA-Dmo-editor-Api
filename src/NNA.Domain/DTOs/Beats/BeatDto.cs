namespace NNA.Domain.DTOs.Beats;

public sealed class BeatDto : BaseDto {
    public string BeatId { get; set; } = null!;
    public string Text { get; set; } = null!;
    public int Order { get; set; }
    public int Type { get; set; }
    public NnaMovieCharacterInBeatDto[] CharactersInBeat { get; set; } = Array.Empty<NnaMovieCharacterInBeatDto>();
    public NnaTagInBeatDto[] TagsInBeat { get; set; } = Array.Empty<NnaTagInBeatDto>();
    public BeatTimeDto Time { get; set; } = null!;
}