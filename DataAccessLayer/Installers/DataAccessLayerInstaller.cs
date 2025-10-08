using DataAccessLayer.Repositories;
using DataAccessLayer.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccessLayer.Installers
{
    public static class DataAccessLayerInstaller
    {
        public static void InstallDataAccessLayerServices(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IWorkGroupRepository, WorkGroupRepository>();
            services.AddScoped<IApplicantRepository, ApplicantRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<ICheckRepository, CheckRepository>();

            services.AddScoped<ITransactionProvider, TransactionProvider>();
        }
    }
}
