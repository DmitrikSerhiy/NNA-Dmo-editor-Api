using System;

namespace API.DTO.DmoCollections {
    public class GetExcludedDmosDto {
        public Guid? CollectionId { get; set; }
        public Boolean Excluded { get; set; }
    }
}
