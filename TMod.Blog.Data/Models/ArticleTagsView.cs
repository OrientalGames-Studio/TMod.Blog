using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TMod.Blog.Data.Models;

[Keyless]
public partial class ArticleTagsView
{
    [StringLength(15)]
    public string Tag { get; set; } = null!;
}
