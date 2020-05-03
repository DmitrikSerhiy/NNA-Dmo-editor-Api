using System;

namespace Model.Entities {
    public class Beat {
        public TimeSpan PlotTimeSpot { get; set; }
        public Int16 Order { get; set; }
        public string Description { get; set; }

        public Dmo Dmo { get; set; }
        public Guid DmoId { get; set; }
    }
}
