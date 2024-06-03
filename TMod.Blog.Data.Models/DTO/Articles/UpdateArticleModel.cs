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
        //[Required]
        public UpdateArticleMetaModel? Meta { get; set; } = null!;

        public List<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();

        public List<ArticleTagViewModel> Tags { get; set; } = new List<ArticleTagViewModel>();
    }

    public sealed class UpdateArticleMetaModel
    {
        public string? Key { get; set; }
        
        public ArticleViewModel? Article { get; set; } = null!;

        
        public ArticleContentViewModel? ArticleContent { get; set; } = null!;
    }
}
