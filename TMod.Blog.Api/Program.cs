using TMod.Blog.Api;
using TMod.Blog.Api.Endpoints;
using TMod.Blog.Api.Tools.ModelBinders;
using TMod.Blog.Data;
using TMod.Blog.Data.Repositories;
using TMod.Blog.Data.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(op =>
{
    op.ModelBinderProviders.Add(new UpdateArticleModelBinderProvider());
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<Constants>();
builder.Services.AddBlogDb()
    .AddBlogRepositories()
    .AddBlogStoreServices();

builder.Services.AddAntiforgery();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        IConfiguration configuration = builder.Configuration.GetSection("Cors");
        IEnumerable<string>? hosts =   configuration.GetSection("Hosts").Get<IEnumerable<string>>();
        IEnumerable<string>? methods = configuration.GetSection("Methods").Get<IEnumerable<string>>();
        IEnumerable<string>? headers = configuration.GetSection("Headers").Get<IEnumerable<string>>();
        policy.WithHeaders(headers?.ToArray() ?? [])
        .WithMethods(methods?.ToArray() ?? [])
        .WithOrigins(headers?.ToArray() ?? [])
        .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if ( app.Environment.IsDevelopment() )
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseAntiforgery();

app.UseCors();

app.MapControllers();

app.MapArticleCommentsEndpoints()
    .MapArticleCategoriesEndpoints()
    .MapArticleTagsEndpoints()
    .MapAntiforgeryTokenEndpoints();

app.Run();
