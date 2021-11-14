using Microsoft.AspNetCore.Hosting;

namespace API.Helpers.Extensions {
    public static class EnvDetector {
        public static bool IsLocal(this IWebHostEnvironment env) {
            return env.EnvironmentName == "Local";
        }
    }
}