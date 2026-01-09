using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Application.Dtos;
using TMod.Blog.Application.Requests.Tag;

namespace TMod.Blog.Application.Services
{
    public interface ITagService
    {
        Task<TagDto> AcquireTagAsync(CreateTagRequest request, CancellationToken token = default);

        Task<IReadOnlyList<TagDto>> AcquireTagsAsync(IEnumerable<CreateTagRequest> requests, CancellationToken token = default);

        Task<IReadOnlyList<TagDto>> GetArticleTagsAsync(Guid articleId, CancellationToken token = default);

        Task<PagingDto<TagDto>> PagingTagsAsync(string? keyword = null, int pageIndex = 1, int pageSize = 20, CancellationToken token = default);

        Task<TagDto?> UpdateTagAsync(Guid tagId, UpdateTagRequest request, CancellationToken token = default);

        Task<bool> DeleteTagAsync(Guid tagId,CancellationToken token = default);
    }
}
