namespace API.Helpers {
    public class ResponseBuilder {

        public object Append401RedirectToLoginMessage() {
            return new { redirectToLogin = true };
        }
        
        public object AppendBadRequestErrorMessage(string errorMessage) {
            return new {errorMessage};
        }

        public object AppendNotFoundErrorMessage(string errorMessage) {
            return new { errorMessage };
        }
    }
}
