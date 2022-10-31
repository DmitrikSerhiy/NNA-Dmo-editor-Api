namespace NNA.Domain.DTOs.Beats;

public class BeatDto : BaseDto {
    public string BeatId { get; set; } = null!;
    public string Text { get; set; } = null!;
    public string Order { get; set; } = null!;
    public int Type { get; set; }
    public NnaMovieCharacterInBeatDto[] Characters { get; set; } = Array.Empty<NnaMovieCharacterInBeatDto>();
    public BeatTimeDto Time { get; set; } = null!;
}