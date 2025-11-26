using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Application.Common.Results;

namespace TMod.Blog.Application.Dtos
{
    public record PagingDto<T>
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int PageCount { get; set; }

        public int DataCount { get; set; }

        public IEnumerable<T> Items { get; set; } = [];

        public static implicit operator Result(PagingDto<T> dto)
        {
            return Result.Paging(Math.Max(0, dto.DataCount), Math.Max(1, dto.PageCount), Math.Max(1, dto.PageIndex), Math.Max(0, dto.PageSize), dto.Items);
        }
    }
}
