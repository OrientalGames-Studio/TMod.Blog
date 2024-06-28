using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Data.Models.DTO.Configuration
{
    public class BatchDeleteConfigurationModel
    {
        [Required]
        public List<int>? ConfigurationIds { get; set; }
    }
}
