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
            config.NewConfig<Tag, TagDto>()
                .Map(dest => dest.ArticleCount, src => src.Articles == null || src.Articles.Count == 0 ? 0 : src.Articles.Count);

            // When mapping from DTO to Entity, keep navigation properties null so EF won't treat them as new entities
            config.NewConfig<TagDto, Tag>()
                .Ignore(dest => dest.Articles);

            // Requests should map to DTOs only
            config.NewConfig<CreateTagRequest, TagDto>();
            config.NewConfig<UpdateTagRequest, TagDto>();
        }
    }
}
