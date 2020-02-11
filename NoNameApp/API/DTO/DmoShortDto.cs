using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTO {
    public sealed class DmoShortDto {
        public String Name { get; set; }
        public String MovieTitle { get; set; }
        public Int16 DmoStatus { get; set; }
        public String ShortComment { get; set; }
        public Int16 Mark { get; set; }
    }
}
