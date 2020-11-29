using Model.Enums;

namespace API.Features.Dmos.Mappers {
    public static class StaticDmoMapper {
        public static string GetDmoStatusString(short status) {
            switch (status)
            {
                case (short)DmoStatus.Complete:
                    return nameof(DmoStatus.Complete);
                case (short)DmoStatus.InProgress:
                    return nameof(DmoStatus.InProgress);
                case (short)DmoStatus.New:
                    return nameof(DmoStatus.New);
                case (short)DmoStatus.NotFinished:
                    return nameof(DmoStatus.NotFinished);
                default:
                    return nameof(DmoStatus.New);
            }
        }
    }
}
