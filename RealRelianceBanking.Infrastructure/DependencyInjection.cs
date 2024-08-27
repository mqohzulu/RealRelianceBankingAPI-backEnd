using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RealRelianceBanking.Application.Common.Interfaces.Authentication;
using RealRelianceBanking.Application.Common.Interfaces.Persistance;
using RealRelianceBanking.Application.Common.Interfaces.Services;
using RealRelianceBanking.Infrastructure.Authentication;
using RealRelianceBanking.Infrastructure.DBContext;
using RealRelianceBanking.Infrastructure.Persistance;
using RealRelianceBanking.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRelianceBanking.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfractracture(this IServiceCollection services, ConfigurationManager configuration)
        {
            services
                .AddAuthentication(configuration)
                .AddContext(configuration)
                .AddPersistence();
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            return services;
      
        }
        public static IServiceCollection AddPersistence(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITransactionRepository, TransactionsRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();

            return services;
        }
        public static IServiceCollection AddAuthentication(this IServiceCollection services, ConfigurationManager configuration)
        {
            var jwtSettings = new JwtSettings();
            configuration.Bind(JwtSettings.SectionName, jwtSettings);

            services.AddSingleton(Options.Create(jwtSettings));
            services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

            services.AddAuthentication(defaultScheme: JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.Secret))
                });
            return services;

        }
        public static IServiceCollection AddContext(this IServiceCollection services, ConfigurationManager configuration)
        {
            var DapperSettings = new DapperSettings();
            configuration.Bind(DapperSettings.SectionName, DapperSettings);
            services.AddSingleton(Options.Create(DapperSettings));
            services.AddSingleton<DapperContext>();

            return services;

        }
    }
}
