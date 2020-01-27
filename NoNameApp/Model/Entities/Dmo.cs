using System;
using Model.Entities.Common;

namespace Model.Entities {
    public sealed class Dmo : Entity
    {
        //public Dmo(NoNameUser user, String name, String movieTitle) {
        //    Name = name;
        //    MovieTitle = movieTitle;
        //    NoNameUser = user;
        //    NoNameUserId = user.Id;
        //}

        public String Name { get; set; }
        public String MovieTitle { get; set; }

        public Guid NoNameUserId { get; set; }
        public NoNameUser NoNameUser { get; set; }
    }
}
