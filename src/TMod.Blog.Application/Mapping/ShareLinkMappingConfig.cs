using Mapster;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Application.Dtos;
using TMod.Blog.Domain.Entities;

namespace TMod.Blog.Application.Mapping
{
    internal class ShareLinkMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<ShareLink, ShareLinkDto>();

            // DTO -> Entity: ignore not-mapped Target property to avoid accidental assignment
            config.NewConfig<ShareLinkDto, ShareLink>()
                .Ignore(dest => dest.Target);
        }
    }
}
