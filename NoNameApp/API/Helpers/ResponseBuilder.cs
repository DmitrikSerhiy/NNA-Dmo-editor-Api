using System;

namespace API.Helpers {
    public class ResponseBuilder {

        public object AppendBadRequestErrorMessage(string errorMessage) {
            return new {errorMessage};
        }
    }
}
