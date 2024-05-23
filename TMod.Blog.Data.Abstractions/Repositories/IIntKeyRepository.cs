using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Data.Interfaces;

namespace TMod.Blog.Data.Abstractions.Repositories
{
    public interface IIntKeyRepository<TModelType> : IRepository<int, TModelType> where TModelType : class, IKey<int>
    {
    }
}
