using System;
using Model.Enums;

namespace API.DTO {
    public sealed class DmoShortDto {
        public String Id { get; set; }
        public String Name { get; set; }
        public String MovieTitle { get; set; }
        public String DmoStatus { get; set; }
        public Int16 DmoStatusId { get; set; }
        public String ShortComment { get; set; }
        public Int16 Mark { get; set; }
    }
}
