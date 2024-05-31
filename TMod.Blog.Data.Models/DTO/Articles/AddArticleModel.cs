using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Data.Models.ViewModels.Articles;

namespace TMod.Blog.Data.Models.DTO.Articles
{
    public sealed class AddArticleModel
    {
        [Required]
        public string Title { get; set; } = null!;

        [Required]
        public string Content { get; set; } = null!;

        public string? Snapshot { get; set; }

        public ArticleStateEnum ArticleState { get; set; } = ArticleStateEnum.Draft;

        public bool IsCommentEnabled { get; set; } = true;

        public List<string> Categories { get; set; } = new List<string>();

        public List<string> Tags { get; set; } = new List<string>();
    }
}
