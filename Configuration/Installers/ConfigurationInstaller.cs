using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Configuration.Installers
{
    public static class ConfigurationInstaller
    {
        public static IServiceCollection ConfigureAppOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            //Add some options here

            return services;
        }
    }
}
