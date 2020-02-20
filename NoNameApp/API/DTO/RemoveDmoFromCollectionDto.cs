using System;

namespace API.DTO
{
    public class RemoveDmoFromCollectionDto {
        public Guid CollectionId { get; set; }
        public Guid DmoId { get; set; }
    }
}
