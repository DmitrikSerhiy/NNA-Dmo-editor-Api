﻿namespace Model.DTOs.Dmos {
    public class EditDmoInfoDto : BaseDto {
        public string Id { get; set; }
        public string Name { get; set; }
        public string MovieTitle { get; set; }
        public string ShortComment { get; set; }
    }
}
