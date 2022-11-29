using NNA.Domain.Enums;

namespace NNA.Domain.DTOs.DmoCollections;

public sealed class DmoDetailsDto: BaseDto {
    public string? Name { get; set; }
    public string MovieTitle { get; set; } = null!;
    public short DmoStatusId { get; set; }
    public string ShortComment { get; set; } = null!;
    
    
    public string? Premise { get; set; }
    public string? ControllingIdea { get; set; }
    public ControllingIdeaType? ControllingIdeaId { get; set; }
    public bool? Didacticism { get; set; }
    public string? DidacticismDescription { get; set; }
    
    // todo: extend for new popup
}
