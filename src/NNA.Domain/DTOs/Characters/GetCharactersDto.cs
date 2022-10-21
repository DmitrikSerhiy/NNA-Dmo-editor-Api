namespace NNA.Domain.DTOs.Characters;

public sealed class GetCharactersDto : BaseDto {
    public string DmoId { get; set; } = null!;
}
