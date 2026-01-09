using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Application.Dtos
{
    public record CategoryDto
    {
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public string? Description { get; set; }

        public Guid? ParentId { get; set; }

        public string? ParentName { get; set; }

        public List<CategoryDto> Children { get; set; } = [];
    }
}
