using System;
using Model.Enums;

namespace API.Helpers {
    public static class StaticDmoMapper {
        public static String GetDmoStatusString(Int16 status) {
            switch (status)
            {
                case (Int16)DmoStatus.Complete:
                    return nameof(DmoStatus.Complete);
                case (Int16)DmoStatus.InProgress:
                    return nameof(DmoStatus.InProgress);
                case (Int16)DmoStatus.New:
                    return nameof(DmoStatus.New);
                case (Int16)DmoStatus.NotFinished:
                    return nameof(DmoStatus.NotFinished);
                default:
                    return nameof(DmoStatus.New);
            }
        }
    }
}
