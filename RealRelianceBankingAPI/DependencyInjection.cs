using RealRelianceBanking.Application.Common.Interfaces.Services;
using RealRelianceBanking.Infrastructure.Services;
using RealRelianceBankingAPI.Common;
using RealRelianceBankingAPI.Services;

namespace RealRelianceBankingAPI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services)
        {
            services.AddControllers();

            services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.Converters.Add(new DateConverter());
                options.SerializerOptions.Converters.Add(new CleanNullableDateConverter());
            });

            services.AddScoped<IEmailService, EmailService>();

            services.AddMappings();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<JwtService>();
            return services;
        }
    }
}
