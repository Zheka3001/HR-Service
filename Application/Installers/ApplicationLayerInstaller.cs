using Application.Services;
using Application.Validators;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Installers
{
    public static class ApplicationLayerInstaller
    {
        public static void InstallApplicationLayerServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IWorkGroupService, WorkGroupService>();
            services.AddScoped<IAuthService, AuthService>();

            services.AddValidation();
        }

        private static void AddValidation(this IServiceCollection services)
        {
            services.AddScoped<IValidationService, ValidationService>();

            services.AddScoped<IValidator, LoginCredentialsValidator>();
            services.AddScoped<IValidator, RegiterUserValiator>();
            services.AddScoped<IValidator, CreateWorkGroupValidator>();

            services.AddScoped(
                provider => new Lazy<IEnumerable<IValidator>>(provider.GetServices<IValidator>));
        }
    }
}
