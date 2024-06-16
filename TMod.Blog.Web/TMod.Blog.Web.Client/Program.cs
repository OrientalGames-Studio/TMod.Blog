using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using MudBlazor;
using MudBlazor.Services;
using MudBlazor.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddMudServices()
    .AddMudExtensions()
    .AddMudMarkdownServices();

await builder.Build().RunAsync();
