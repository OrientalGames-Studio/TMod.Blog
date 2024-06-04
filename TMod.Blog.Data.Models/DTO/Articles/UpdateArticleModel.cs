using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Data.Models.ViewModels.Articles;
using TMod.Blog.Data.Models.ViewModels.Categories;

namespace TMod.Blog.Data.Models.DTO.Articles
{
    public sealed class UpdateArticleModel
    {
        [Required]
        public UpdateArticleMetaModel? Meta { get; set; }

        public List<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();

        public List<ArticleTagViewModel> Tags { get; set; } = new List<ArticleTagViewModel>();
    }

    public sealed class UpdateArticleMetaModel
    {
        [Required]
        public ArticleViewModel? Article { get; set; }

        [Required]
        public ArticleContentViewModel? ArticleContent { get; set; }

        public List<ArticleArchiveViewModel> Archives { get; set; } = new List<ArticleArchiveViewModel>();
    }
}
