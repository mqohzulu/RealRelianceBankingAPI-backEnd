using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RealRelianceBanking.Application.Common.Behaviors;

namespace RealRelianceBanking.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssemblyContaining<DependencyInjection>();
            });
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            return services;
        }
    }
}
