using BlazorClient;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
var environment = builder.HostEnvironment.Environment;
if (!string.IsNullOrWhiteSpace(environment))
{
    builder.Configuration.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: false);
}
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration["WebApiAddress"]) });
builder.Services.AddBlazorBootstrap();
builder.Services.AddServiceRegistration(builder.Configuration);
await builder.Build().RunAsync();
