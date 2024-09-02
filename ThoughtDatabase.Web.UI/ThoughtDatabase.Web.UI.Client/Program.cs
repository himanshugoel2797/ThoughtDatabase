using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazored.LocalStorage;
using ThoughtDatabase.Web.UI.Client;
using ThoughtDatabase.Web;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
var baseAddr = new UriBuilder(builder.HostEnvironment.BaseAddress) { Port = 7083 }.Uri; // Use UriBuilder to set the port
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddTransient(sp => new HttpClient { BaseAddress = baseAddr });
ServiceCollectionManager.RegisterServices(builder.Services);

await builder.Build().RunAsync();
