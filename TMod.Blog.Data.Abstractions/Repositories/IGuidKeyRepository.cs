using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Data.Interfaces;

namespace TMod.Blog.Data.Abstractions.Repositories
{
    public interface IGuidKeyRepository<TModelType> : IRepository<Guid, TModelType> where TModelType : class, IKey<Guid>
    {
    }
}
