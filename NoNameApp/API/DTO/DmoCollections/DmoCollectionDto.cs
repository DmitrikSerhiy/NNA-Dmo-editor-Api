namespace API.DTO.DmoCollections {
    public class DmoCollectionDto {
        public string Id { get; set; }
        // ReSharper disable once UnusedMember.Global
        public string CollectionName { get; set; }
        // ReSharper disable once UnusedMember.Global
        public int DmoCount { get; set; }
        public DmoShortDto[] Dmos { get; set; }
    }
}
