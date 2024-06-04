using System.ComponentModel.DataAnnotations;

using TMod.Blog.Data.Interfaces;

namespace TMod.Blog.Data.Models.ViewModels.Articles
{
    public class ArticleArchiveViewModel : IGuidKey, IKey<Guid>
    {
        /// <summary>
        /// 附件主键标识
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 文章外键标识
        /// </summary>
        public Guid ArticleId { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        [StringLength(30)]
        public string? ArchiveName { get; set; }
              
        /// <summary>
        /// 文件大小
        /// </summary>
        public double ArchiveFileSize { get; set; }

        /// <summary>
        /// 附件MIME类型
        /// </summary>
        [StringLength(100)]
        public string ArchiveMimetype { get; set; } = "application/octet-stream";

        /// <summary>
        /// 编辑时，附件是否标记为删除
        /// </summary>
        public bool IsRemove { get; set; } = false;

        public static implicit operator ArticleArchive? (ArticleArchiveViewModel? viewModel)
        {
            if(viewModel is null )
            {
                return null;
            }
            ArticleArchive archive = new ArticleArchive()
            {
                Id = viewModel.Id,
                ArticleId = viewModel.ArticleId,
                ArchiveName = viewModel.ArchiveName,
                ArchiveFileSize = viewModel.ArchiveFileSize,
                ArchiveMimetype = viewModel.ArchiveMimetype,
            };
            return archive;
        }

        public static implicit operator ArticleArchiveViewModel? (ArticleArchive? archive)
        {
            if(archive is null )
            {
                return null;
            }
            ArticleArchiveViewModel viewModel = new ArticleArchiveViewModel()
            {
                Id = archive.Id,
                ArticleId = archive.ArticleId,
                ArchiveName = archive.ArchiveName,
                ArchiveFileSize = archive.ArchiveFileSize,
                ArchiveMimetype = archive.ArchiveMimetype
            };
            return viewModel;
        }
    }
}
