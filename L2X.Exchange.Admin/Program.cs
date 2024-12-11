var builder = WebApplication.CreateBuilder(args);

var startup = new L2X.Exchange.Admin.Startup(builder.Configuration);

startup.ConfigureServices(builder.Services);

var app = builder.Build();

startup.Configure(app, app.Environment);

app.MapControllers();
//app.MapRazorPages();

app.Run();