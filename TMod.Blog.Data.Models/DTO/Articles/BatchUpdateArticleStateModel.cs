﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Data.Models.ViewModels.Articles;

namespace TMod.Blog.Data.Models.DTO.Articles
{
    public sealed class BatchUpdateArticleStateModel
    {
        [Required]
        public Dictionary<Guid, ArticleStateEnum> ArticleStates { get; set; } = new Dictionary<Guid, ArticleStateEnum>();
    }
}
