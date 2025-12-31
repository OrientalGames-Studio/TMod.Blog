using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Infrastructure.Contextes
{
    public class TMod_Blog_DesignTime : IDesignTimeDbContextFactory<TMod_Blog_RW_Context>
    {
        public TMod_Blog_RW_Context CreateDbContext(string[] args)
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddUserSecrets(Assembly.GetExecutingAssembly())
                .AddUserSecrets("24b27681-456b-485a-afdb-730282c2a68d");

            var configuration = configurationBuilder.Build();

            // 首先尝试从 ConnectionStrings:DefaultConnection 获取，其次尝试默认键 DefaultConnection
            var connectionString = configuration.GetConnectionString("TMod.Blog_RW") ?? configuration["TMod.Blog_RW"];

            if ( string.IsNullOrWhiteSpace(connectionString) )
            {
                throw new InvalidOperationException(
                    "未找到连接字符串。请在 user secrets 或 环境变量 中设置 'ConnectionStrings:TMod.Blog_RW' 或 'TMod.Blog_RW'.");
            }

            var optionsBuilder = new DbContextOptionsBuilder<TMod_Blog_RW_Context>();
            optionsBuilder.UseSqlServer(connectionString);

            return new TMod_Blog_RW_Context(optionsBuilder.Options);
        }
    }
}
