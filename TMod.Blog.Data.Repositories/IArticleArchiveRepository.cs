using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Data.Abstractions.Repositories;
using TMod.Blog.Data.Models;

namespace TMod.Blog.Data.Repositories
{
    public interface IArticleArchiveRepository:IGuidKeyRepository<ArticleArchive>
    {
        IEnumerable<ArticleArchive?> GetArticleArchivesByArticleId(Guid articleId);

        Stream? GetArticleArchiveContent(Guid articleId, Guid archiveId, out string? archiveName, out string? mimeType, out double? fileSize);
    }
}
