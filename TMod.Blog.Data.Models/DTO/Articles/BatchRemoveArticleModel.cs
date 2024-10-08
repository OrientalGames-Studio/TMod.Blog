﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Data.Models.DTO.Articles
{
    public sealed class BatchRemoveArticleModel
    {
        [Required]
        public List<Guid> ArticleIds { get; set; } = new List<Guid>();
    }
}
