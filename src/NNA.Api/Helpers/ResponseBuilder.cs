namespace NNA.Api.Helpers;
public static class ResponseBuilder {
    public static object AppendBadRequestErrorMessage(string errorMessage) {
        return new { errorMessage };
    }

    public static object AppendBadRequestErrorMessageToForm(string errorMessage) {
        return new { errorMessage, hasMassageInUI = true };
    }

    public static NnaValidationResult AppendValidationErrorMessage(string field, string errorMessage) {
        return new NnaValidationResult {
            Fields = new[] { new NnaValidationResultFields { Field = field, Errors = new[] { errorMessage } } }
        };
    }
}

