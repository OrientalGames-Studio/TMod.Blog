using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Data.Abstractions.Repositories;
using TMod.Blog.Data.Models;

namespace TMod.Blog.Data.Repositories
{
    public interface ICategoryRepository:IIntKeyRepository<Category>
    {
        Category? GetCategoryByCategoryName(string category);

        Task BatchRemoveCategoryByIdAsync(params int[] categoryIds);
    }
}
