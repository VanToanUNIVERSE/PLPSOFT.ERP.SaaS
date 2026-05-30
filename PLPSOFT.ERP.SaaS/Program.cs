using PLPSOFT.ERP.SaaS.Modules.CRM.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddCrmInfrastructure(builder.Configuration);

// builder.Services.AddScoped<PLPSOFT.ERP.SaaS.Modules.CRM.Application.Interfaces.IScheduleService, PLPSOFT.ERP.SaaS.Modules.CRM.Infrastructure.Services.ScheduleService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
