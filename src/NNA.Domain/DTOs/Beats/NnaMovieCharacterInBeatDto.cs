namespace NNA.Domain.DTOs.Beats;

public sealed class NnaMovieCharacterInBeatDto : BaseDto {
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}
