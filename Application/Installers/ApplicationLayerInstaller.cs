using Application.Models;
using Application.Services;
using Application.Services.Interfaces;
using Application.Validators;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;

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
            services.AddScoped<IPriciplesFromTokenProvider, PriciplesFromTokenProvider>();
            services.AddScoped<IApplicantService, ApplicantService>();
            services.AddScoped<ICheckService, CheckService>();

            services.AddSingleton<JwtSecurityTokenHandler>();

            services.AddValidation();
        }

        private static void AddValidation(this IServiceCollection services)
        {
            services.AddScoped<IValidationService, ValidationService>();

            services.AddScoped<IValidator, LoginCredentialsValidator>();
            services.AddScoped<IValidator, RegisterUserValiator>();
            services.AddScoped<IValidator, CreateWorkGroupValidator>();
            services.AddScoped<IValidator, MoveHrsRequestValidator>();
            services.AddScoped<IValidator, CreateApplicantRequestValidator>();
            services.AddScoped<IValidator<SocialNetwork>, SocialNetworkValidator>();
            services.AddScoped<IValidator, UpdateApplicantRequestValidator>();

            services.AddScoped(
                provider => new Lazy<IEnumerable<IValidator>>(provider.GetServices<IValidator>));
        }
    }
}
