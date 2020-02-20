using System;

namespace API.DTO {
    public class AddDmoToCollectionDto {
        public Guid CollectionId { get; set; }
        public Guid DmoId { get; set; }
    }
}
