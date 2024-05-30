using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

using TMod.Blog.Data.Interfaces;

namespace TMod.Blog.Data.Models;

/// <summary>
/// 博客的文章标签表，用来存储博客的标签
/// </summary>
public partial class ArticleTag:IIntKey
{
    /// <summary>
    /// 自增主键
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// 文章外键标识
    /// </summary>
    public Guid ArticleId { get; set; }

    /// <summary>
    /// 文章标签
    /// </summary>
    [StringLength(15)]
    public string Tag { get; set; } = null!;

    [ForeignKey("ArticleId")]
    [InverseProperty("ArticleTags")]
    public virtual Article Article { get; set; } = null!;
}
