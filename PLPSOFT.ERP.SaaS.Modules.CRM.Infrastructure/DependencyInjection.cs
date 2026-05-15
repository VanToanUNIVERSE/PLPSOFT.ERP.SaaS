using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PLPSOFT.ERP.SaaS.Modules.CRM.Application.Interfaces;
using PLPSOFT.ERP.SaaS.Modules.CRM.Infrastructure.Persistence;
using PLPSOFT.ERP.SaaS.Modules.CRM.Infrastructure.Services;

namespace PLPSOFT.ERP.SaaS.Modules.CRM.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCRMInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<CRMDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            // Đăng ký các Service
            services.AddScoped<IScheduleService, ScheduleService>();

            return services;
        }
    }
}
