using Microsoft.EntityFrameworkCore;
using Asaf.EFCoreAudit.Configurations;
using Asaf.EFCoreAudit.Interceptors;
using Asaf.AuditLog.Web.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHostedService<AppDbMigratorService>();
builder.Services.AddDbContext<AppDbContext>((sp, options) =>
{
    var interceptor = sp.GetRequiredService<IAuditInterceptor>();
    options.AddInterceptors(interceptor);
    options.UseInMemoryDatabase("product_db");
});


builder.Services.AddAudit(settings =>
{
    settings.AddedEnabled = true;
    settings.AddEntityFrameworkProvider(oprions =>
    {
        oprions.UseSqlite(@"Data Source=AppData\audit.db");
    });

    //settings.AddMongoDbProvider(@"connectionstring");
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseAudit();

app.Run();