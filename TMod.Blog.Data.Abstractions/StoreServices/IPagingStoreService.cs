using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Data.Abstractions.StoreServices
{
    public interface IPagingStoreService<TModel> where TModel : class
    {
        IQueryable<TModel?> Paging(int pageIndex,int pageSize,out int totalDataCount,out int totalPageCount, Func<TModel?, bool>? filter = null);
        IQueryable<TModel?> Paging(IEnumerable values, int pageIndex, int pageSize, out int totalDataCount, out int totalPageCount, Func<TModel?, bool>? filter = null);
    }
}
