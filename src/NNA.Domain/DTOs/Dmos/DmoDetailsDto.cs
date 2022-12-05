using NNA.Domain.Enums;

namespace NNA.Domain.DTOs.Dmos;

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

    public DmoCharactersForConflictDto[] CharactersForConflict { get; set; } = Array.Empty<DmoCharactersForConflictDto>();
    public DmoConflictDto[] Conflicts { get; set; } = Array.Empty<DmoConflictDto>();
    

}
