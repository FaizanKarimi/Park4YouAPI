using Microsoft.AspNetCore.Builder;

namespace ParkForYouAPI.Middlewares
{
    public static class ApplicationMiddlewares
    {
        public static IApplicationBuilder UseCustomMessageHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomMessageHandler>();
        }
    }
}