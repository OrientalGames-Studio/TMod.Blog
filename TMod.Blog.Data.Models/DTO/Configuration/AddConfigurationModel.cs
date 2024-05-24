using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Data.Models.DTO.Configuration
{
    public sealed class AddConfigurationModel
    {
        [Required]
        public string ConfigurationKey { get; set; } = null!;

        public object? ConfigurationValue { get; set; }
    }
}
