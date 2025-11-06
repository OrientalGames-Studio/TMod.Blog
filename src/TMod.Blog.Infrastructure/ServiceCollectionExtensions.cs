using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Domain.Entities;
using TMod.Blog.Domain.Interfaces;
using TMod.Blog.Domain.Interfaces.Repositories;
using TMod.Blog.Domain.Interfaces.UnitOfWorks;
using TMod.Blog.Infrastructure.Repositories;
using TMod.Blog.Infrastructure.Repositories.Aggregates;
using TMod.Blog.Infrastructure.Repositories.UnitOfWorks;

namespace TMod.Blog.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBlogInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, ApplicationUnitOfWork>();
            services.AddScoped<IApplicationUnitOfWork,ApplicationUnitOfWork>();

            services.AddScoped(typeof(IReadOnlyRepository<,>), typeof(ReadOnlyBlogRepository<,>));
            services.AddScoped(typeof(IRepository<,>), typeof(BlogRepository<,>));

            services.AddScoped<IArticleRepository, ArticleRepository>();
            services.AddScoped<IReadOnlyRepository<Article, Guid>, ArticleRepository>();
            services.AddScoped<IRepository<Article, Guid>, ArticleRepository>();

            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IReadOnlyRepository<Category, Guid>, CategoryRepository>();
            services.AddScoped<IRepository<Category, Guid>, CategoryRepository>();

            services.AddScoped<ICommentRepository,CommentRepository>();
            services.AddScoped<IReadOnlyRepository<Comment, Guid>, CommentRepository>();
            services.AddScoped<IRepository<Comment,Guid>, CommentRepository>();

            services.AddScoped<IShareLinkRepository, ShareLinkRepository>();
            services.AddScoped<IReadOnlyRepository<ShareLink,int>, ShareLinkRepository>();
            services.AddScoped<IRepository<ShareLink,int>, ShareLinkRepository>();

            services.AddScoped<ISiteConfigurationRepository, SiteConfigurationRepository>();
            services.AddScoped<IReadOnlyRepository<SiteConfiguration, int>, SiteConfigurationRepository>();
            services.AddScoped<IRepository<SiteConfiguration, int>, SiteConfigurationRepository>();

            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<IReadOnlyRepository<Tag,Guid>,TagRepository>();
            services.AddScoped<IRepository<Tag,Guid>, TagRepository>();
            return services;
        }
    }
}
