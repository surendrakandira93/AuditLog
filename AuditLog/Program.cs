using AuditLog.Auditing;
using AuditLog.Core.EntityHistory;
using AuditLog.Data;
using AuditLog.Service;
using Microsoft.EntityFrameworkCore;
using AuditLog.Data.EntityHistory;
using AuditLog.Core.Auditing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using AuditLog.Data.Auditing;
using Microsoft.AspNetCore.Mvc.Filters;
using AuditLog.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<EmployeeDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddControllersWithViews();
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<AuditActionFilter>();
    options.Filters.Add<AuditPageFilter>();
    options.Filters.Add<ExceptionFilter>();
    options.Filters.Add<AbpExceptionPageFilter>();
});

builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
builder.Services.AddTransient<IEmployeeService, EmployeeService>();
builder.Services.AddTransient<IDepartmentService, DepartmentService>();
builder.Services.AddTransient<IAuditLogService, AuditLogService>();
builder.Services.AddTransient<IEntityChangeService, EntityChangeService>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

#region EntityHistore
builder.Services.AddSingleton<IClientInfoProvider, HttpContextClientInfoProvider>();
builder.Services.AddScoped<IEntityHistoryStore, EntityHistoryStore>();
builder.Services.AddScoped<IEntityHistoryHelper, EntityHistoryHelper>();
#endregion

#region Auditing
builder.Services.AddTransient<IAuditSerializer, JsonNetAuditSerializer>();
builder.Services.AddTransient<IAuditingHelper, AuditingHelper>();
builder.Services.AddTransient<IAuditInfoProvider, DefaultAuditInfoProvider>();
builder.Services.AddTransient<IAuditingStore, AuditingStore>();
builder.Services.AddSingleton<IErrorInfoBuilder, ErrorInfoBuilder>();
builder.Services.AddSingleton<IExceptionToErrorInfoConverter, DefaultErrorInfoConverter>();
#endregion


//Configure MVC
//builder.Services.Configure<MvcOptions>(mvcOptions => { 
//    mvcOptions.Filters.AddService(typeof(AuditActionFilter));
//    mvcOptions.Filters.AddService(typeof(AuditPageFilter));
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
