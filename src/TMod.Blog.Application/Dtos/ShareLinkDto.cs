using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Domain.Entities;

namespace TMod.Blog.Application.Dtos
{
    public record ShareLinkDto
    {
        public int Id { get; set; }

        public ShareTargetTypeEnum TargetType { get; set; }

        public Guid TargetId { get; set; }

        public string? ShortCode { get; set; }

        public string? CreatorIp { get; set; }
    }
}
