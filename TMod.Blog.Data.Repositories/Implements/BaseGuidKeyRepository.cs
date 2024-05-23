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
    public abstract class BaseGuidKeyRepository<TModel> : BaseRepository<Guid, TModel>, IRepository<Guid, TModel> where TModel : class, IKey<Guid>
    {
        protected BaseGuidKeyRepository(BlogContext blogContext, ILoggerFactory loggerFactory) : base(blogContext, loggerFactory)
        {
        }
    }
}
