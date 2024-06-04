using TMod.Blog.Api;
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if ( app.Environment.IsDevelopment() )
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
