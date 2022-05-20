using CheckInQrWeb.Core;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddTransient<CombinedParser>();

var app = builder.Build();
app.UseRouting();
app.MapControllers();
app.Run();
