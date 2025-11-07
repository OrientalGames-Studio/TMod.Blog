using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Application.Requests.SiteConfiguration
{
    public record CreateConfigurationRequest
    {
        public required string ConfigKey { get; set; }

        public string? ConfigValue { get; set; }

        public string? Description { get; set; }
    }
}
