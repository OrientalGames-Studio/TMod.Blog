using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Domain.Entities
{
    public class Tag : BaseEntity<Guid>
    {
        [MaxLength(64)]
        public required string Name { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }

        public virtual ICollection<Article> Articles { get; set; } = [];
    }
}
