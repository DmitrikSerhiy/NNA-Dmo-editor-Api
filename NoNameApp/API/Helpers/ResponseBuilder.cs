namespace API.Helpers {
    public class ResponseBuilder {
        public object AppendBadRequestErrorMessage(string errorMessage) {
            return new { errorMessage };
        }
        
        public object AppendBadRequestErrorMessageToForm(string errorMessage) {
            return new { errorMessage, hasMassageInUI = true };
        }

        public NnaValidationResult AppendValidationErrorMessage(string field, string errorMessage) {
            return new NnaValidationResult {
                Fields = new[] { new NnaValidationResultFields { Field = field, Errors = new[] { errorMessage } } }
            };
        }
    }
}
