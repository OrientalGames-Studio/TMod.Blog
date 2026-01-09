using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Domain.Interfaces.Services
{
    public interface ISiteConfigurationCacheService
    {
        string? GetConfiguration(string key);

        void Refresh();
    }
}
