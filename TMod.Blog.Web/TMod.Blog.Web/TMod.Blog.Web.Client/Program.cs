using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using MudBlazor.Services;
using MudBlazor.Extensions;
using MudBlazor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddMudServices()
    .AddMudPopoverService()
    .AddMudExtensions()
    .AddMudMarkdownServices();

await builder.Build().RunAsync();
