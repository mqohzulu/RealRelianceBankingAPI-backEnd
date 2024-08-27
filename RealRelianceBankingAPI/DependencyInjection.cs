using RealRelianceBankingAPI.Common;
using RealRelianceBankingAPI.Services;

namespace RealRelianceBankingAPI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddMappings();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<JwtService>();
            return services;
        }
    }
}
