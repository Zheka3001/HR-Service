using Configuration.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Configuration.Installers
{
    public static class ConfigurationInstaller
    {
        public static IServiceCollection ConfigureAppOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services
                .Configure<JwtTokenOptions>(configuration.GetSection(JwtTokenOptions.Name));

            return services;
        }
    }
}
