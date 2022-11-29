using NNA.Domain.Enums;

namespace NNA.Domain.DTOs.Dmos;

public sealed class UpdateDmoPlotDetailsDto: BaseDto {
    public string? Premise { get; set; }
    public string? ControllingIdea { get; set; }
    public ControllingIdeaType ControllingIdeaId { get; set; }
    public bool? Didacticism { get; set; }
    public string? DidacticismDescription { get; set; }
}
