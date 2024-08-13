using TMod.Blog.Web.Components;
using MudBlazor;
using MudBlazor.Services;
using MudBlazor.Extensions;

using TMod.Blog.Web.Services;
using TMod.Blog.Web.Endpoints;
using TMod.Blog.Web.Interactive;
using System.Text;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddMudServices()
    .AddMudExtensions()
    .AddMudMarkdownServices();


builder.Services.AddHttpClient("apiClient", c =>
{
    c.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ApiServiceUrl") ?? throw new InvalidOperationException("请先配置服务接口地址再注入HttpClient"));
    c.DefaultRequestHeaders.AcceptEncoding.Add(StringWithQualityHeaderValue.Parse(new UTF8Encoding().BodyName));
    c.DefaultRequestHeaders.AcceptCharset.Add(StringWithQualityHeaderValue.Parse(new UTF8Encoding().BodyName));
});

builder.Services.AddIconPathProviderService()
    .AddLocalStorageProviderService()
    .AddAppConfigurationProviderService() 
    .AddAdminNavMenuProviderService()
    .AddCategoryService();

var app = builder.Build();

// Configure the HTTP request pipeline.
if ( app.Environment.IsDevelopment() )
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(TMod.Blog.Web.Client._Imports).Assembly,typeof(TMod.Blog.Web.Core._Imports).Assembly);

app.MapLocalConfigurationEndpoint();

app.Run();
