using Mapster;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Application.Dtos;
using TMod.Blog.Application.Requests.Article;
using TMod.Blog.Domain.Entities;

namespace TMod.Blog.Application.Mapping
{
    internal class ArticleMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Article, ArticleDto>()
                .Map(dest => dest.CategoryName, src => src.Category != null ? src.Category.Name : "未分类")
                .Map(dest=>dest.Tags,src=>src.Tags == null || src.Tags.Count == 0? new List<string>() : src.Tags.Select(t=>t.Name))
                .TwoWays();

            config.NewConfig<CreateArticleRequest, ArticleDto>();
            config.NewConfig<UpdateArticleRequest, ArticleDto>();
        }
    }
}
