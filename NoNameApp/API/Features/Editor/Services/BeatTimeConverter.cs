using System;

namespace API.Features.Editor.Services {
    public class BeatTimeConverter {

        public static string DefaultTimeView { get; } = "0:00:00";
        
        public static string ConvertBeatTimeToTimeView(int timeInSeconds) {
            var timeSpanResult = TimeSpan.FromSeconds(timeInSeconds);
            return timeSpanResult.ToString("g").Substring(0, 6);
        }
    }
}