using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Application.Dtos;
using TMod.Blog.Application.Requests.Article;
using TMod.Blog.Domain.Entities;

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
        /// <param name="articleId">文章 Id</param>
        /// <param name="updateArticleRequest">修改文章请求入参</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ArticleDto?> UpdateArticleAsync(Guid articleId, UpdateArticleRequest updateArticleRequest, CancellationToken cancellationToken = default);

        /// <summary>
        /// 修改文章分类
        /// </summary>
        /// <param name="articleId">文章 Id</param>
        /// <param name="patchArticleCategoryRequest">修改文章分类入参</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ArticleDto?> PatchArticleCategoryAsync(Guid articleId, PatchArticleCategoryRequest patchArticleCategoryRequest,CancellationToken cancellationToken = default);

        /// <summary>
        /// 修改文章是否允许评论
        /// </summary>
        /// <param name="articleId">文章 Id</param>
        /// <param name="request">请求入参</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ArticleDto?> PatchArticleIsCommentEnabledAsync(Guid articleId, PatchArticleIsCommentEnabledRequest request,CancellationToken cancellationToken = default);

        /// <summary>
        /// 修改文章是否允许分享
        /// </summary>
        /// <param name="articleId">文章 Id</param>
        /// <param name="request">请求入参</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ArticleDto?> PatchArticleIsShareEnabledAsync(Guid articleId, PatchArticleIsShareEnabledRequest request,CancellationToken cancellationToken = default);

        /// <summary>
        /// 修改文章标签
        /// </summary>
        /// <param name="articleId">文章 Id</param>
        /// <param name="request">请求入参</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ArticleDto?> PatchArticleTagsAsync(Guid articleId, PatchArticleTagsRequest request,CancellationToken cancellationToken = default);

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
        /// <param name="categoryId">分类Id</param>
        /// <param name="tagId">标签Id</param>
        /// <param name="keyword">文章标题或Slug关键字</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">单页数据量</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<PagingDto<ArticleDto>> PagingArticleAsync(Guid? categoryId = null,Guid? tagId = null,string? keyword = null,ArticleStatusEnum articleStatus = ArticleStatusEnum.Draft| ArticleStatusEnum.Published| ArticleStatusEnum.Unpublished , int pageIndex = 1,int pageSize = 20,CancellationToken cancellationToken = default);
    }
}
