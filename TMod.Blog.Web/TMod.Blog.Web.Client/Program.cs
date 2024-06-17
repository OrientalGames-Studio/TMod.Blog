using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using MudBlazor;
using MudBlazor.Services;
using MudBlazor.Extensions;

using TMod.Blog.Web.Services;
using TMod.Blog.Web.Interactive;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddMudServices()
    .AddMudExtensions()
    .AddMudMarkdownServices();

builder.Services.AddHttpClient("localClient", c =>
{
    c.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
});

builder.Services.AddHttpClient("apiClient", async (provider, client) =>
{
    IHttpClientFactory httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
    HttpClient localClient = httpClientFactory.CreateClient("localClient");
    string? apiServiceUrl = await localClient.GetStringAsync("api/v1/configurations/ApiServiceUrl");
    client.BaseAddress = new Uri(apiServiceUrl ?? throw new InvalidOperationException("请先配置服务接口地址再注入HttpClient"));
});

builder.Services.AddIconPathProviderService()
    .AddAppConfigurationProviderService();

await builder.Build().RunAsync();
