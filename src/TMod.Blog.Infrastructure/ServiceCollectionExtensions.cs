using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Domain.Entities;
using TMod.Blog.Domain.Interfaces;
using TMod.Blog.Domain.Interfaces.Repositories;
using TMod.Blog.Infrastructure.Repositories;
using TMod.Blog.Infrastructure.Repositories.Aggregates;

namespace TMod.Blog.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBlogInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IReadOnlyRepository<,>), typeof(ReadOnlyBlogRepository<,>));
            services.AddScoped(typeof(IRepository<,>), typeof(BlogRepository<,>));

            services.AddScoped<IArticleRepository, ArticleRepository>();
            services.AddScoped<IReadOnlyRepository<Article, Guid>, ArticleRepository>();
            services.AddScoped<IRepository<Article, Guid>, ArticleRepository>();
            return services;
        }
    }
}
