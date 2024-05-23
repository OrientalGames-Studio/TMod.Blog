﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Data.Interfaces;

namespace TMod.Blog.Data.Abstractions.Repositories
{
    public interface IRepository<TKeyType, TModelType> where TModelType : class,IKey<TKeyType>
    {
        Task<IEnumerable<TModelType>> GetAllAsync();

        Task<TModelType> CreateAsync(TModelType model);

        Task<TModelType> UpdateAsync(TModelType model);

        Task RemoveAsync(TModelType model);

        Task RemoveAsync(TKeyType key);

        Task<TModelType?> LoadAsync(TKeyType key);
    }
}
