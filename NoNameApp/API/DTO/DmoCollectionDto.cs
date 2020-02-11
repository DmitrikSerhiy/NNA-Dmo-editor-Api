using System;

namespace API.DTO {
    public class DmoCollectionDto {
        public String CollectionName { get; set; }
        public Int32 DmoCount { get; set; }
        public DmoShortDto[] Dmos { get; set; }
    }
}
