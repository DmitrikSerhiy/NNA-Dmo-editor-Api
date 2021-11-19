namespace API.Helpers {
    public class ResponseBuilder {
        
        // todo: move to headers
        public object AppendBadRequestErrorMessage(string errorMessage) {
            return new { errorMessage };
        }
    }
}
