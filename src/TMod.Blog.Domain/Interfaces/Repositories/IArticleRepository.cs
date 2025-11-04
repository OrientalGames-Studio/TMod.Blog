using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Domain.Entities;

namespace TMod.Blog.Domain.Interfaces.Repositories
{
    /// <summary>
    /// 文章聚合根仓储接口
    /// </summary>
    public interface IArticleRepository : IRepository<Article, Guid>, IReadOnlyRepository<Article, Guid>
    {
        /// <summary>
        /// 获取文章及其关联标签、分类、评论
        /// </summary>
        /// <param name="id">文章编号</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Article?> GetArticleWithDetailByIdAsync(Guid id,CancellationToken cancellationToken = default);

        /// <summary>
        /// 按照分类分页查询文章
        /// </summary>
        /// <param name="categoryId">分类Id</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">单页数据大小</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IReadOnlyList<Article>> PagingArticleByCategoryAsync(Guid categoryId, int pageIndex = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    }
}
