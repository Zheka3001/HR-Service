using Application.Services;
using Application.Validators;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Installers
{
    public static class ApplicationLayerInstaller
    {
        public static void InstallApplicationLayerServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();

            services.AddValidation();
        }

        private static void AddValidation(this IServiceCollection services)
        {
            services.AddScoped<IValidationService, ValidationService>();

            services.AddScoped<IValidator, LoginCredentialsValidator>();
            services.AddScoped<IValidator, RegiterUserValiator>();

            services.AddScoped(
                provider => new Lazy<IEnumerable<IValidator>>(provider.GetServices<IValidator>));
        }
    }
}
