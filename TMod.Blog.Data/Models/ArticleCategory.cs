using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

using TMod.Blog.Data.Interfaces;

namespace TMod.Blog.Data.Models;

/// <summary>
/// 博客的文章内容表，用来存储博客的正文内容
/// </summary>
public partial class ArticleCategory: IIntKey, IKey<int>
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
    /// 分类外键标识
    /// </summary>
    public int CategoryId { get; set; }

    [ForeignKey("ArticleId")]
    [InverseProperty("ArticleCategories")]
    public virtual Article Article { get; set; } = null!;

    [ForeignKey("CategoryId")]
    [InverseProperty("ArticleCategories")]
    public virtual Category Category { get; set; } = null!;
}
