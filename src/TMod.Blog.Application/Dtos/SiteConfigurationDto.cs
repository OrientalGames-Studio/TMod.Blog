using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Application.Dtos
{
    public record SiteConfigurationDto
    {
        public int Id { get; set; }

        public required string ConfigKey { get; set; }

        public string? ConfigValue { get; set; }

        public string? Description { get; set; }

        public bool IsEnabled { get; set; } = true;
    }
}
