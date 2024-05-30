using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Data.Models;

namespace TMod.Blog.Data.Repositories.Implements
{
    internal class ArticleArchiveRepository : BaseGuidKeyRepository<ArticleArchive>, IArticleArchiveRepository
    {
        public ArticleArchiveRepository(BlogContext blogContext, ILoggerFactory loggerFactory) : base(blogContext, loggerFactory)
        {
        }

        public Stream? GetArticleArchiveContent(Guid articleId, Guid archiveId, out string? archiveName, out string? mimeType, out double? fileSize)
        {
            archiveName = null;
            mimeType = null;
            fileSize = null;
            Article? article = base.BlogContext.Articles.Include(p=>p.ArticleArchives).FirstOrDefault(p=>p.Id == articleId);
            if(article is null )
            {
                return null;
            }
            ArticleArchive? archive = article.ArticleArchives.FirstOrDefault(p=>p.Id == archiveId);
            if(archive is null || (archive.ArchiveContent is null || archive.ArchiveContent.Length <= 0))
            {
                return null;
            }
            archiveName = archive.ArchiveName;
            mimeType = archive.ArchiveMimetype;
            fileSize = archive.ArchiveFileSize;
            return new MemoryStream(archive.ArchiveContent);
        }

        public IEnumerable<ArticleArchive?> GetArticleArchivesByArticleId(Guid articleId)
        {
            Article? article = base.BlogContext.Articles.FirstOrDefault(p=>p.Id == articleId);
            if(article is null )
            {
                yield break;
            }
            foreach ( ArticleArchive archive in base.BlogContext.ArticleArchives.Where(p=>p.ArticleId == articleId) )
            {
                yield return archive;
            }
        }
    }
}
