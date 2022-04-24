namespace Model.DTOs.Editor {
    public class LoadedShortDmoDto : BaseDto {
        // ReSharper disable InconsistentNaming
        public string id { get; set; }
        public string name { get; set; }
        public string movieTitle { get; set; }
        public string shortComment { get; set; }
        public bool hasBeats { get; set; }
        public short dmoStatus { get; set; }
    }
}
