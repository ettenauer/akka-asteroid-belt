using AsteroidBelt.Web;
using AsteroidBelt.Web.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureLogging((configLogging) => configLogging.AddConsole());
builder.Services.AddSignalR();
builder.Services.AddTransient<AsteroidHubAdapter>();
builder.Services.AddHostedService<AkkaService>();
builder.Services.AddLogging();
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.UseStaticFiles();
app.MapHub<AsteroidHub>("/hubs/asteroidHub");
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

await app.RunAsync().ConfigureAwait(false);
