using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Data.Abstractions.Repositories;
using TMod.Blog.Data.Interfaces;

namespace TMod.Blog.Data.Repositories.Implements
{
    public abstract class BaseIntKeyRepository<TModel> : BaseRepository<int, TModel>, IRepository<int, TModel> where TModel : class, IKey<int>
    {
        protected BaseIntKeyRepository(BlogContext blogContext, ILoggerFactory loggerFactory) : base(blogContext, loggerFactory)
        {
        }
    }
}
