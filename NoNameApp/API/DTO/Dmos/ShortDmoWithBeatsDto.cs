namespace API.DTO.Dmos {
    public class ShortDmoWithBeatsDto {
        public string Id { get; set; }
        public string Name { get; set; }
        // ReSharper disable once UnusedMember.Global
        public string MovieTitle { get; set; }
        public string DmoStatus { get; set; }
        public short DmoStatusId { get; set; }
        // ReSharper disable once UnusedMember.Global
        public string ShortComment { get; set; }
        // ReSharper disable once UnusedMember.Global
        public short Mark { get; set; }
        public BeatDto[] Beats { get; set; }
    }
}
