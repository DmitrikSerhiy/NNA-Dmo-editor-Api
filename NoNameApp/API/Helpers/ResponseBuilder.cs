using System;

namespace API.Helpers {
    public class ResponseBuilder {

        public object AppendBadRequestErrorMessage(string errorMessage) {
            //todo: relocate this logic to appropriate filter
            return new {errorMessage};
        }
    }
}
