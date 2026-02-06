using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Domain.Entities;
using TMod.Blog.Domain.Specifications;

namespace TMod.Blog.Infrastructure.Specifications
{
    public sealed class ArticleSpecification : BaseSpecification<Article>
    {
        public ArticleSpecification()
        {
        }

        public ArticleSpecification(Expression<Func<Article, bool>> criteria) : base(criteria)
        {
        }

        public static ISpecification<Article> CreateGetDetail(Guid articleId,bool showDeleted = false)
        {
            ArticleSpecification specification = new ArticleSpecification(a=>a.Id == articleId);
            specification.AddInclude(c => c.Category);
            specification.AddInclude(c => c.Tags);
            //specification.AddInclude(c => c.Comments);
            if ( !showDeleted )
            {
                specification.AddCriteria(c => !c.IsDeleted);
            }
            return specification;
        }

        public static ISpecification<Article> CreatePagingByCategoryId(Guid categoryId, int skip, int take, string? keyword = null,ArticleStatusEnum articleStatus = ArticleStatusEnum.Draft | ArticleStatusEnum.Published | ArticleStatusEnum.Unpublished, bool showDeleted = false)
        {
            ArticleSpecification specification = new ArticleSpecification(a=>a.CategoryId == categoryId);
            specification.ApplyPaging(skip, take);
            specification.AddInclude(c => c.Category);
            specification.AddInclude(c => c.Tags);
            specification.AddCriteria(a => ( a.Status & articleStatus ) == articleStatus);
            if ( !string.IsNullOrWhiteSpace(keyword) )
            {
                specification.AddCriteria(a=>EF.Functions.Like(a.Title, $"%{keyword}%") || ( a.Slug != null && EF.Functions.Like(a.Slug, $"%{keyword}%") ));
            }
            if ( !showDeleted )
            {
                specification.AddCriteria(c => !c.IsDeleted);
            }
            return specification;
        }

        public static ISpecification<Article> CreatePaging(int skip, int take, string? keyword = null, ArticleStatusEnum articleStatus = ArticleStatusEnum.Draft | ArticleStatusEnum.Published | ArticleStatusEnum.Unpublished, bool showDeleted = false)
        {
            ArticleSpecification specification = new ArticleSpecification();
            specification.ApplyPaging(skip, take);
            specification.AddInclude(c => c.Category);
            specification.AddInclude(c => c.Tags);
            specification.AddCriteria(a => ( a.Status & articleStatus ) == articleStatus);
            if ( !string.IsNullOrWhiteSpace(keyword) )
            {
                specification.AddCriteria(a => EF.Functions.Like(a.Title, $"%{keyword}%") || (a.Slug != null && EF.Functions.Like(a.Slug, $"%{keyword}%") ));
            }
            if ( !showDeleted )
            {
                specification.AddCriteria(c => !c.IsDeleted);
            }
            return specification;
        }

        public static ISpecification<Article> CreateCountSlug(string slug)
        {
            ArticleSpecification articleSpecification = new ArticleSpecification(a=>a.Slug.IsSimilarTo(slug,0.85));
            articleSpecification.AddCriteria(a => !a.IsDeleted);
            return articleSpecification;
        }

        public static ISpecification<Article> CreateGetBySlug(string slug,bool showDeleted = false)
        {
            ArticleSpecification specification = new ArticleSpecification(a=>a.Slug!=null && a.Slug.Equals(slug));
            specification.AddInclude(c => c.Category);
            specification.AddInclude(c => c.Tags);
            specification.AddInclude(c => c.Comments);
            if ( !showDeleted )
            {
                specification.AddCriteria(c => !c.IsDeleted);
            }
            return specification;
        }

        public static ISpecification<Article> CreatePagingWithFullFilter(int skip,int take,Guid? categoryId = null,Guid? tagId = null,string? keyword = null,ArticleStatusEnum articleStatus = ArticleStatusEnum.Draft|ArticleStatusEnum.Published|ArticleStatusEnum.Unpublished,bool showDeleted = false)
        {
            ArticleSpecification specification = new ArticleSpecification();
            specification.AddInclude(a => a.Category);
            specification.AddInclude(a => a.Tags);
            if (categoryId is not null && categoryId != Guid.Empty )
            {
                specification.AddCriteria(a=>a.CategoryId == categoryId);
            }
            if(tagId is not null && tagId != Guid.Empty )
            {
                specification.AddCriteria(a=>a.Tags.Any(t=>t.Id == tagId));
            }
            if ( !string.IsNullOrWhiteSpace(keyword) )
            {
                specification.AddCriteria(a => EF.Functions.Like(a.Title,$"%{keyword}%") || (a.Slug != null && EF.Functions.Like(a.Slug,$"%{keyword}%")));
            }
            if ( !showDeleted )
            {
                specification.AddCriteria(a => !a.IsDeleted);
            }
            specification.AddCriteria(a=>(a.Status & articleStatus ) == a.Status);
            specification.ApplyPaging(skip, take);
            return specification;
        }

        public static ISpecification<Article> CreateCountForPreparePaging(Guid? categoryId = null, Guid? tagId = null, string? keyword = null, ArticleStatusEnum articleStatus = ArticleStatusEnum.Draft | ArticleStatusEnum.Published | ArticleStatusEnum.Unpublished, bool showDeleted = false)
        {
            ArticleSpecification specification = new ArticleSpecification();
            if ( categoryId is not null && categoryId != Guid.Empty )
            {
                specification.AddInclude(a => a.Category);
                specification.AddCriteria(a => a.CategoryId == categoryId);
            }
            if ( tagId is not null && tagId != Guid.Empty )
            {
                specification.AddInclude(a => a.Tags);
                specification.AddCriteria(a => a.Tags.Any(t => t.Id == tagId));
            }
            if ( !string.IsNullOrWhiteSpace(keyword) )
            {
                specification.AddCriteria(a => EF.Functions.Like(a.Title, $"%{keyword}%") || ( a.Slug != null && EF.Functions.Like(a.Slug, $"%{keyword}%") ));
            }
            if ( !showDeleted )
            {
                specification.AddCriteria(a => !a.IsDeleted);
            }
            specification.AddCriteria(a => ( a.Status & articleStatus ) == a.Status);
            return specification;
        }
    }
}
