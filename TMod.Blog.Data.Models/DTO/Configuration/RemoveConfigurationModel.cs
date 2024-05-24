using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Data.Models.DTO.Configuration
{
    public sealed class RemoveConfigurationModel
    {
        public int? ConfigurationId { get; set; }

        public string? ConfigurationKey { get; set; }
    }
}
