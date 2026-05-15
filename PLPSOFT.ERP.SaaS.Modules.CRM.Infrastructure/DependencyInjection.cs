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
        public static IServiceCollection AddCrmInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Đăng ký DbContext
            services.AddDbContext<CrmDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));

            // Đăng ký Services
            services.AddScoped<IFeedbackService, FeedbackService>();
            services.AddScoped<IScheduleService, ScheduleService>();

            return services;
        }
    }
}
