using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Domain.Entities;

namespace TMod.Blog.Domain.Interfaces.Repositories
{
    /// <summary>
    /// 标签聚合根仓储接口
    /// </summary>
    public interface ITagRepository:IRepository<Tag,Guid>,IReadOnlyRepository<Tag, Guid>
    {
        /// <summary>
        /// 通过名称获取标签
        /// </summary>
        /// <param name="name">标签名称</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Tag?> GetByNameAsync(string? name, CancellationToken cancellationToken = default);

        /// <summary>
        /// 按照标签分页查询文章
        /// </summary>
        /// <param name="tagId">标签Id</param>
        /// <param name="keyword">文章标题或Slug模糊查询</param>
        /// <param name="articleStatus">文章状态筛选</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">单页数据大小</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IReadOnlyList<Article>> PagingArticleByTagAsync(Guid tagId, string? keyword = null, ArticleStatusEnum articleStatus = ArticleStatusEnum.Draft | ArticleStatusEnum.Published | ArticleStatusEnum.Unpublished, int pageIndex = 1, int pageSize = 20, CancellationToken cancellationToken = default);

        /// <summary>
        /// 分页查询标签
        /// </summary>
        /// <param name="keyword">标签名称模糊查询</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">单页数据大小</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IReadOnlyList<Tag>> PagingTagsAsync(string? keyword = null, int pageIndex = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    }
}
