using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Domain.Entities;
using TMod.Blog.Domain.Specifications;

namespace TMod.Blog.Infrastructure.Specifications
{
    internal class ShareLinkSpecification : BaseSpecification<ShareLink>
    {
        public static ISpecification<ShareLink> CreateGetShareLinkByShortCode(string shortCode)
        {
            ShareLinkSpecification result = new ShareLinkSpecification();
            result.AddCriteria(s => s.ShortCode == shortCode);
            return result;
        }
    }
}
