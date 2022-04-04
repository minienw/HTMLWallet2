using CheckInQrWeb.Core;
using CheckInQrWeb.Core.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});
builder.Services.AddAuthentication(
        CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();
builder.Services.AddHttpClient();

//builder.Services.AddTransient<GetValidationServiceDescriptionDataCommand>();
//builder.Services.AddTransient<ProcessStartDccValidationRequestCommand>();
builder.Services.AddTransient<HttpGetIdentityCommand>();
builder.Services.AddTransient<HttpPostTokenCommand>();
builder.Services.AddTransient<HttpPostValidateCommand>();
builder.Services.AddTransient<VerificationWorkflow>();
builder.Services.AddTransient<HttpPostCallbackCommand>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
