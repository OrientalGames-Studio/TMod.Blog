using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Data.Models.DTO.Categories
{
    public sealed class BatchDeleteCategoryModel
    {
        [Required]
        public List<int>? CategoryIds { get; set; }
    }
}
