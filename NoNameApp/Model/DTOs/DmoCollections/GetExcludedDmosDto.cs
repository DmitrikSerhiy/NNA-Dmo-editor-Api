using System;

namespace Model.DTOs.DmoCollections {
    public class GetExcludedDmosDto {
        public Guid? CollectionId { get; set; }
        // ReSharper disable once UnusedMember.Global
        public bool Excluded { get; set; }
    }
}
