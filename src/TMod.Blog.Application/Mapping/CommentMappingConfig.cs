using Mapster;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Application.Dtos;
using TMod.Blog.Application.Requests.Comment;
using TMod.Blog.Domain.Entities;

namespace TMod.Blog.Application.Mapping
{
    internal class CommentMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Comment, CommentDto>()
                .Map(dest => dest.AuthorIp, src => src.AuthorIp)
                .Map(dest => dest.IsShareEnabled, src => src.IsShareEnabled)
                .Map(dest => dest.CommentDate,src => DateOnly.FromDateTime(src.CreateDate))
                .Map(dest => dest.CommentTime,src => TimeOnly.FromDateTime(src.CreateDate));

            // When mapping from DTO to Entity, keep navigation properties null so EF won't treat them as new entities
            config.NewConfig<CommentDto, Comment>()
                .Ignore(dest => dest.Article)
                .Ignore(dest => dest.Parent)
                .Ignore(dest => dest.Replies);

            // Requests should map to DTOs only
            config.NewConfig<CreateCommentRequest, CommentDto>();
            config.NewConfig<UpdateCommentRequest, CommentDto>();
        }
    }
}
