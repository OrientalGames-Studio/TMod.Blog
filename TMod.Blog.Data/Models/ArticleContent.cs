using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TMod.Blog.Data.Models;

/// <summary>
/// 博客的文章内容表，用来存储博客的正文内容
/// </summary>
public partial class ArticleContent
{
    /// <summary>
    /// 自增主键
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// 文章编号外键
    /// </summary>
    public Guid ArticleId { get; set; }

    /// <summary>
    /// 文章正文内容
    /// </summary>
    public string Content { get; set; } = null!;

    [ForeignKey("ArticleId")]
    [InverseProperty("ArticleContents")]
    public virtual Article Article { get; set; } = null!;
}
