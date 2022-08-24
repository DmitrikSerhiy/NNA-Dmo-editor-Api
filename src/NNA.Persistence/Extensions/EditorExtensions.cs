using Microsoft.Data.SqlClient;

namespace NNA.Persistence.Extensions;
public static class EditorExtensions {
    public static void Initialize(this SqlCommand command, string commandText, object parameters) {
        command.CommandText = commandText;
        var properties = parameters.GetType().GetProperties();

        foreach (var parameter in properties) {
            command.Parameters.Add(new SqlParameter(parameter.Name, parameter.GetValue(parameters)));
        }
    }
}