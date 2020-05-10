using Serilog;
using System;

namespace Model.Exceptions {
    public class UpdateBeatsWSException : Exception {
        public UpdateBeatsWSException() {
            Log.Error("Failed to update beats json");
        }
    }
}
