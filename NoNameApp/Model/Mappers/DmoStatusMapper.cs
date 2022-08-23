using Model.Enums;

namespace Model.Mappers; 
public static class DmoStatusMapper {
    public static string GetDmoStatusString(short status) {
        return status switch {
            (short)DmoStatus.InProgress => "In progress",
            (short)DmoStatus.Completed => "Completed",
            (short)DmoStatus.NotFinished => "Not finished",
            _ => "In progress"
        };
    }
}