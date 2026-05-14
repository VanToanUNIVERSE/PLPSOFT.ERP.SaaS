using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PLPSOFT.ERP.SaaS.Modules.CRM.Infrastructure.Persistence;

namespace PLPSOFT.ERP.SaaS.Modules.CRM.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCrmInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<CrmDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("CrmDatabase")));

            return services;
        }
    }
}
