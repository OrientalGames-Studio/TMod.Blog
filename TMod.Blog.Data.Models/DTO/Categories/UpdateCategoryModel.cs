using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Data.Models.DTO.Categories
{
    public sealed class UpdateCategoryModel
    {
        [Required]
        [StringLength(20)]
        public string? Category { get; set; } = null!;
    }
}
