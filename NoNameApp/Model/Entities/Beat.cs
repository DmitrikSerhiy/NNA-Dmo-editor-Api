using System;
using Model.Entities.Common;

namespace Model.Entities {
    public class Beat : Entity {
        public TimeSpan PlotTimeSpot { get; set; }
        // ReSharper disable once UnusedMember.Global
        public short Order { get; set; }
        // ReSharper disable once UnusedMember.Global

        public string Description { get; set; }

        public Dmo Dmo { get; set; }
        public Guid DmoId { get; set; }
    }
}
