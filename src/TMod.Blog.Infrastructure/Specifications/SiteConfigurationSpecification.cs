using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Domain.Entities;
using TMod.Blog.Domain.Specifications;

namespace TMod.Blog.Infrastructure.Specifications
{
    public sealed class SiteConfigurationSpecification : BaseSpecification<SiteConfiguration>
    {
        public SiteConfigurationSpecification()
        {
        }

        public SiteConfigurationSpecification(Expression<Func<SiteConfiguration, bool>> criteria) : base(criteria)
        {
        }

        public static ISpecification<SiteConfiguration> CreateGetConfiguration(string configurationKey,bool asNoTracking = true, bool showDeleted = false)
        {
            SiteConfigurationSpecification specification = new SiteConfigurationSpecification(c=>StringComparer.InvariantCulture.Compare(configurationKey,c.ConfigKey) == 0);
            specification.AddCriteria(c => c.IsEnabled == true);
            if ( !showDeleted )
            {
                specification.AddCriteria(c => !c.IsDeleted);
            }
            if ( asNoTracking )
            {
                specification.EnabledNoTracking();
            }
            return specification;
        }

        public static ISpecification<SiteConfiguration> CreatePaging(string? keyword, int skip, int take,bool showDisabled = false, bool showDeleted = false)
        {
            SiteConfigurationSpecification specification = new SiteConfigurationSpecification();
            if ( !string.IsNullOrWhiteSpace(keyword) )
            {
                specification.AddCriteria(c => c.ConfigKey.Contains(keyword, StringComparison.InvariantCultureIgnoreCase));
            }
            if ( !showDisabled )
            {
                specification.AddCriteria(c => c.IsEnabled);
            }
            if ( !showDeleted )
            {
                specification.AddCriteria(c=>!c.IsDeleted);
            }
            specification.ApplyPaging(skip, take);
            specification.ApplyOrderBy(c => c.ConfigKey);
            specification.ApplyThenBy(c => c.IsEnabled);
            return specification;
        }
    }
}
