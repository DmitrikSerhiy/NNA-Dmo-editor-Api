using Model.Enums;

namespace Model.Mappers {
    public static class DmoStatusMapper {
        public static string GetDmoStatusString(short status) {
            switch (status)
            {
                case (short)DmoStatus.InProgress:
                    return nameof(DmoStatus.InProgress);
                case (short)DmoStatus.Completed:
                    return nameof(DmoStatus.Completed);
                case (short)DmoStatus.NotFinished:
                    return nameof(DmoStatus.NotFinished);
                default:
                    return nameof(DmoStatus.InProgress);
            }
        }
    }
}
