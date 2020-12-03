using Model.Enums;

namespace Model.Mappers {
    public static class DmoStatusMapper {
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
