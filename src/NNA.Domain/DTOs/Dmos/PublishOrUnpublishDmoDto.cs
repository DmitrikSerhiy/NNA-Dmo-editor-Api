using NNA.Domain.Enums;

namespace NNA.Domain.DTOs.Dmos;

public sealed class PublishOrUnpublishDmoDto: BaseDto {
    public DmoPublishState State { get; set; }  
}