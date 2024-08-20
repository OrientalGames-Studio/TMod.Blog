using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Web.Models.Articles
{
    public sealed class QueryArticleFilterModel
    {
        public string? ArticleTitleFilter { get; set; }

        public string? ArticleSnapshotFilter { get; set; }

        public IEnumerable<string?> ArticleCategoryFilter { get; set; } = Enumerable.Empty<string?>();

        public IEnumerable<string?> ArticleTagFilter { get; set; } = Enumerable.Empty<string?>();

        public DateOnly? MinArticleCreateDate { get; set; }

        public DateOnly? MaxArticleCreateDate { get; set; }

        public DateOnly? MinArticleLastEditDate { get; set; }

        public DateOnly? MaxArticleLastEditDate { get; set; }

        public DateOnly? MinArticlePublishedDate { get; set; }

        public DateOnly? MaxArticlePublishedDate { get; set; }

        public short? ArticleStateFilter { get; set; }

        public bool? ArticleCommentStateFilter { get; set; }
    }
}
