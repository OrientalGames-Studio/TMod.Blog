using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using MudBlazor;
using MudBlazor.Services;
using MudBlazor.Extensions;

using TMod.Blog.Web.Services;
using System.Text.Json;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddMudServices()
    .AddMudExtensions()
    .AddMudMarkdownServices();

HttpClient startupClient = new HttpClient();
startupClient.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
string? apiServiceUrl = await startupClient.GetStringAsync("/api/v1/configurations/ApiServiceUrl");
JsonDocument jsonDocument = JsonDocument.Parse(apiServiceUrl);
apiServiceUrl = jsonDocument.RootElement.GetString();
builder.Services.AddKeyedSingleton<string>("apiServiceUrl", apiServiceUrl??"");
builder.Services.AddHttpClient<HttpClient, HttpClient>("localClient", factory: (client, provider) => startupClient);

builder.Services
    .AddHttpClient("apiClient", (provider, c) =>
    {
        string serviceUrl = provider.GetRequiredKeyedService<string>("apiServiceUrl");
        ILogger<Program> logger = provider.GetRequiredService<ILogger<Program>>();
        logger.LogCritical(serviceUrl);
        c.BaseAddress = new Uri(serviceUrl);
    });

builder.Services.AddIconPathProviderService()
    .AddAppConfigurationProviderService();

await builder.Build().RunAsync();
