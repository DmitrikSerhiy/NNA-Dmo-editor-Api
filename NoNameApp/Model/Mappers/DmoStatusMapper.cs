using Model.Enums;

namespace Model.Mappers {
    public static class DmoStatusMapper {
        public static string GetDmoStatusString(short status) {
            return status switch {
                (short)DmoStatus.InProgress => nameof(DmoStatus.InProgress),
                (short)DmoStatus.Completed => nameof(DmoStatus.Completed),
                (short)DmoStatus.NotFinished => nameof(DmoStatus.NotFinished),
                _ => nameof(DmoStatus.InProgress)
            };
        }
    }
}
