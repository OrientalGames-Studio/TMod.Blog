using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Application.Dtos;
using TMod.Blog.Application.Requests.Article;

namespace TMod.Blog.Application.Services
{
    /// <summary>
    /// 文章服务接口
    /// </summary>
    public interface IArticleService
    {
        /// <summary>
        /// 创建文章
        /// </summary>
        /// <param name="createArticleRequest">创建文章请求入参</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ArticleDto> CreateArticleAsync(CreateArticleRequest createArticleRequest, CancellationToken cancellationToken = default);

        /// <summary>
        /// 修改文章
        /// </summary>
        /// <param name="updateArticleRequest">修改文章请求入参</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ArticleDto?> UpdateArticleAsync(UpdateArticleRequest updateArticleRequest, CancellationToken cancellationToken = default);

        /// <summary>
        /// 修改文章分类
        /// </summary>
        /// <param name="patchArticleCategoryRequest">修改文章分类入参</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ArticleDto?> PatchArticleCategoryAsync(PatchArticleCategoryRequest patchArticleCategoryRequest,CancellationToken cancellationToken = default);

        /// <summary>
        /// 修改文章是否允许评论
        /// </summary>
        /// <param name="request">请求入参</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ArticleDto?> PatchArticleIsCommentEnabledAsync(PatchArticleIsCommentEnabledRequest request,CancellationToken cancellationToken = default);

        /// <summary>
        /// 修改文章是否允许分享
        /// </summary>
        /// <param name="request">请求入参</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ArticleDto?> PatchArticleIsShareEnabledAsync(PatchArticleIsShareEnabledRequest request,CancellationToken cancellationToken = default);

        /// <summary>
        /// 修改文章标签
        /// </summary>
        /// <param name="request">请求入参</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ArticleDto?> PatchArticleTagsAsync(PatchArticleTagsRequest request,CancellationToken cancellationToken = default);

        /// <summary>
        /// 删除文章
        /// </summary>
        /// <param name="articleId">文章编号</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> DeleteArticleAsync(Guid articleId,CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取文章详细数据（分类、评论、标签）
        /// </summary>
        /// <param name="articleId">文章编号</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ArticleDto?> GetArticleDetailAsync(Guid articleId,CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取文章详细数据（分类、评论、标签）
        /// </summary>
        /// <param name="slug">文章 Slug</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ArticleDto?> GetArticleDetailAsync(string slug,CancellationToken cancellationToken = default);

        /// <summary>
        /// 分页查询文章
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="tagId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<PagingDto<ArticleDto>> PagingArticleAsync(Guid? categoryId = null,Guid? tagId = null,int pageIndex = 1,int pageSize = 20,CancellationToken cancellationToken = default);
    }
}
