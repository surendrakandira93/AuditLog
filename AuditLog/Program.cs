using AuditLog.Auditing;
using AuditLog.Core;
using AuditLog.Data.Auditing;
using AuditLog.Core.EntityHistory;
using AuditLog.Data;
using AuditLog.Service;
using Microsoft.EntityFrameworkCore;
using AuditLog.Data.EntityHistory;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllersWithViews();
builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));

builder.Services.AddSingleton<IClientInfoProvider, HttpContextClientInfoProvider>();
builder.Services.AddScoped<IEntityHistoryStore, EntityHistoryStore>();
builder.Services.AddScoped<IEntityHistoryHelper, EntityHistoryHelper>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<IEmployeeService, EmployeeService>();
builder.Services.AddTransient<IDepartmentService, DepartmentService>();
builder.Services.AddTransient<IAuditLogService, AuditLogService>();
builder.Services.AddTransient<IEntityChangeService, EntityChangeService>();

builder.Services.AddDbContext<EmployeeDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


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
