using System;
using System.Collections.Generic;
using System.Text;

using TMod.Blog.Application.Common.Enums;

namespace TMod.Blog.Application.Requests.Share
{
    public record CreateShareRequest
    {
        public bool AutoExpire { get; set; }

        public DaysUnitEnum DaysUnit { get; set; }

        public int ExpireAt { get; set; }
    }
}
