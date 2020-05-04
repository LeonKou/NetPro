using System;

namespace NetPro.Checker
{
    /// <summary>
    /// Represents a health check response
    /// </summary>
    public struct HealthResponse
    {
        public readonly bool IsHealthy;
        public readonly object Response;

        private HealthResponse(bool isHealthy, object statusObject)
        {
            IsHealthy = isHealthy;
            Response = statusObject;
        }

        public static HealthResponse Healthy()
        {
            return Healthy("OK");
        }

        public static HealthResponse Healthy(object response)
        {
            return new HealthResponse(true, response);
        }

        public static HealthResponse Unhealthy()
        {
            return Unhealthy("FAILED");
        }

        public static HealthResponse Unhealthy(object response)
        {
            return new HealthResponse(false, response);
        }

        public static HealthResponse Unhealthy(Exception exception)
        {
            var message = $"EXCEPTION: {exception.GetType().Name}, {exception.Message}";
            return new HealthResponse(false, message);
        }

    }
}
