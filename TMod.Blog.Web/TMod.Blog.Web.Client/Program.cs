using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using MudBlazor;
using MudBlazor.Services;
using MudBlazor.Extensions;

using TMod.Blog.Web.Services;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Text;

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
        c.BaseAddress = new Uri(serviceUrl);
        c.DefaultRequestHeaders.AcceptEncoding.Add(StringWithQualityHeaderValue.Parse(new UTF8Encoding().BodyName));
        c.DefaultRequestHeaders.AcceptCharset.Add(StringWithQualityHeaderValue.Parse(new UTF8Encoding().BodyName));
        //c.DefaultRequestHeaders.Append(new KeyValuePair<string, IEnumerable<string>>("Access-Control-Allow-Origin", ["*"]));
        //c.DefaultRequestHeaders.TryAddWithoutValidation("Access-Control-Allow-Origin", "*");
    });

builder.Services.AddIconPathProviderService()
    .AddLocalStorageProviderService()
    .AddAppConfigurationProviderService()
    .AddAdminNavMenuProviderService();

await builder.Build().RunAsync();
