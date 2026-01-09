using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Domain.Entities;

namespace TMod.Blog.Domain.Interfaces.Repositories
{
    /// <summary>
    /// 分类聚合根仓储接口
    /// </summary>
    public interface ICategoryRepository:IRepository<Category,Guid>,IReadOnlyRepository<Category,Guid>
    {
        /// <summary>
        /// 按照父分类分页查询子分类
        /// </summary>
        /// <param name="parentId">父分类编号</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">单页数据量</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IReadOnlyList<Category>> PagingCategoriesByParentIdAsync(Guid? parentId,int pageIndex = 1,int pageSize = 20,CancellationToken cancellationToken = default); 
    }
}
