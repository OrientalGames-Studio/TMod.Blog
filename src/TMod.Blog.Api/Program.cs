using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using System.Text.Json.Serialization;

using TMod.Blog.Infrastructure.Contextes;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    //options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

builder.Services.AddDbContextPool<TMod_Blog_RW_Context>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("TMod.Blog_RW"));
});

var app = builder.Build();

app.Run();
