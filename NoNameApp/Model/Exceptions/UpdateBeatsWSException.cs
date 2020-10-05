using Serilog;
using System;

namespace Model.Exceptions {
    // ReSharper disable once UnusedMember.Global
    public class UpdateBeatsWsException : Exception {
        public UpdateBeatsWsException() {
            Log.Error("Failed to update beats json");
        }
    }
}
