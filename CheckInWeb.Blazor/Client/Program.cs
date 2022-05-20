using CheckInWeb.Blazor;
using CheckInWeb.Blazor.Core;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

namespace Company.WebApplication1
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddMudServices();
            //builder.Services.Configure<CookiePolicyOptions>(options =>
            //{
            //    options.CheckConsentNeeded = context => true;
            //    options.MinimumSameSitePolicy = SameSiteMode.None;
            //});
            builder.Logging.SetMinimumLevel(LogLevel.Debug); //TODO set from env

            builder.Services.AddHttpClient();
            builder.Services.AddTransient<HttpGetIdentityCommand>();
            builder.Services.AddTransient<HttpPostTokenCommand>();
            builder.Services.AddTransient<HttpPostValidateCommand>();
            builder.Services.AddTransient<HttpPostCallbackCommand>();
            builder.Services.AddTransient<VerificationWorkflow>();
            //var app = builder.Build();

            //// Configure the HTTP request pipeline.
            //if (!app.Environment.IsDevelopment())
            //{
            //    app.UseExceptionHandler("/Error");
            //    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //    app.UseHsts();
            //}

            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //    //app.UseBrowserLink();
            //}

            //app.UseHttpsRedirection();
            //app.UseStaticFiles();
            //app.UseRouting();
            //app.MapControllers();
            //app.UseAuthentication();
            //app.UseAuthorization();
            //app.MapBlazorHub();
            //app.MapFallbackToPage("/_Host");
            //app.Run();

            await builder.Build().RunAsync();
        }
    }
}