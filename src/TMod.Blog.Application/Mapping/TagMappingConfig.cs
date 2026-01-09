using Mapster;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Application.Dtos;
using TMod.Blog.Application.Requests.Tag;
using TMod.Blog.Domain.Entities;

namespace TMod.Blog.Application.Mapping
{
    internal class TagMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Tag,TagDto>()
                .Map(dest=>dest.ArticleCount,src=>src.Articles == null || src.Articles.Count == 0?0:src.Articles.Count)
                .TwoWays();

            config.NewConfig<CreateTagRequest, TagDto>();
            config.NewConfig<UpdateTagRequest, TagDto>();
        }
    }
}
