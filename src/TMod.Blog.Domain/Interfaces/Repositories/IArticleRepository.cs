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
        /// <param name="keyword">文章标题或Slug模糊查询</param>
        /// <param name="articleStatus">文章状态筛选</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">单页数据大小</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IReadOnlyList<Article>> PagingArticleByCategoryAsync(Guid categoryId,string? keyword = null, ArticleStatusEnum articleStatus = ArticleStatusEnum.Draft|ArticleStatusEnum.Published|ArticleStatusEnum.Unpublished, int pageIndex = 1, int pageSize = 20, CancellationToken cancellationToken = default);

        /// <summary>
        /// 计算一个 SEO 友好的 Slug 出现的次数
        /// </summary>
        /// <param name="slug">字符串</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> CountSlugAsync(string slug,CancellationToken cancellationToken = default);

        /// <summary>
        /// 通过 Slug 获取文章
        /// </summary>
        /// <param name="slug">Slug</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Article?> GetArticleBySlugAsync(string slug,CancellationToken cancellationToken = default);
    }
}
