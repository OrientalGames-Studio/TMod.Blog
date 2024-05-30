using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Data.Models.ViewModels.Articles
{
    public sealed class ArticleArchiveContentViewModel
    {
        public Guid ArchiveId { get; set; }

        public string ArchiveName { get; set; } = null!;

        public string MIMEType { get; set; } = "application/octet-stream";

        public Stream? ArchiveData { get; set; }

        public double FileSize { get; set; } = 0d;
    }
}
