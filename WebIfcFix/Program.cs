using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebIfcFix;
using WebIfcFix.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<PredefinedLayoutsService>();
builder.Services.AddSingleton<IJsonConvertService, JsonConvertService>();
builder.Services.AddSingleton<ILocalStorageService, LocalStorageService>();
builder.Services.AddSingleton<IComponentsTypesService, ComponentsTypesService>();
builder.Services.AddSingleton<ISanitizerService, SanitizerService>();
builder.Services.AddSingleton<IUrlQueryParameterService, UrlQueryService>();
builder.Services.AddSingleton<LayoutManagerService>();

await builder.Build().RunAsync();
