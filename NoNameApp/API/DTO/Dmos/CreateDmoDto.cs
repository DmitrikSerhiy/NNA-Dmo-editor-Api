using System;

namespace API.DTO.Dmos {
    public class CreateDmoDto {
        public String Name { get; set; }
        public String MovieTitle { get; set; }
        public String ShortComment { get; set; }
        public Int16 Mark { get; set; }
    }
}
