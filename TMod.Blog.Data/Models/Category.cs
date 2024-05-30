using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TMod.Blog.Data.Models;

/// <summary>
/// 博客的文章分类表，用来存储博客可以选择的分类
/// </summary>
[Index("Category1", Name = "Uk__Categories_Category", IsUnique = true)]
public partial class Category
{
    /// <summary>
    /// 自增主键
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// 分类标签
    /// </summary>
    [Column("Category")]
    [StringLength(20)]
    public string Category1 { get; set; } = null!;

    /// <summary>
    /// 是否已删除
    /// </summary>
    public bool IsRemove { get; set; }

    /// <summary>
    /// 分类版本
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// 创建日期
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    /// <summary>
    /// 修改日期
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime? UpdateDate { get; set; }

    /// <summary>
    /// 删除日期
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime? RemoveDate { get; set; }

    [InverseProperty("Category")]
    public virtual ICollection<ArticleCategory> ArticleCategories { get; set; } = new List<ArticleCategory>();
}
