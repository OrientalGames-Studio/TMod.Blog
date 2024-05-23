using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Data
{
    public static class BlogDbExtensions
    {
        public static IServiceCollection AddBlogDb(this IServiceCollection services)
        {
            services.AddDbContext<BlogContext>((provider, builder) =>
            {
                IConfiguration configuration = provider.GetRequiredService<IConfiguration>();
                builder.UseSqlServer(configuration.GetConnectionString("TMod.Blog.Db"));
            });
            return services;
        }
    }
}
