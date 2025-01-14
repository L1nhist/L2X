var builder = WebApplication.CreateBuilder(args);

var startup = new L2X.Exchange.Site.Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();
startup.Configure(app, app.Environment);

app.MapControllers();
app.MapControllerRoute(
    name: "action",
    pattern: "${type}",
    defaults: new { controller = "Home", action = "Index" });
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();