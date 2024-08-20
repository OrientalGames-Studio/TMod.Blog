using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Web.Interactive.Abstraction;
using TMod.Blog.Web.Services.Abstraction;

namespace TMod.Blog.Web.Services.Client
{
    internal class TagService : ITagService
    {
        private readonly ITagApi _tagApi;
        private readonly ILogger<TagService> _logger;

        public TagService(ITagApi tagApi, ILogger<TagService> logger)
        {
            _tagApi = tagApi;
            _logger = logger;
        }

        public Task<IEnumerable<string?>> GetTagsAsync()=>_tagApi.GetTagsAsync();
    }
}
