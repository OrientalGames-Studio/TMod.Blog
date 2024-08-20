using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

using TMod.Blog.Data.Interfaces;

namespace TMod.Blog.Data.Models;

/// <summary>
/// 博客文章主表，用来存储博客的文章快照（标题、状态、简讯等）
/// </summary>
public partial class Article:IGuidKey,IVersionControl,ICreate,IUpdate,IRemove,IEdit,IKey<Guid>
{
    /// <summary>
    /// Uid 主键标识
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// 文章标题
    /// </summary>
    [StringLength(60)]
    public string Title { get; set; } = null!;

    /// <summary>
    /// 文章前150个字内容节选
    /// </summary>
    [StringLength(150)]
    public string? Snapshot { get; set; }

    /// <summary>
    /// 文章状态（1：草稿，2：已发布，4：已隐藏）
    /// </summary>
    public short State { get; set; }

    /// <summary>
    /// 文章是否允许评论
    /// </summary>
    public bool IsCommentEnabled { get; set; }

    /// <summary>
    /// 最后一次编辑文章日期
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime? LastEditDate { get; set; }

    /// <summary>
    /// 文章首次发表日期
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime? PublishDate { get; set; }

    /// <summary>
    /// 是否已删除
    /// </summary>
    public bool IsRemove { get; set; }

    /// <summary>
    /// 文章版本
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// 文章创建日期
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    /// <summary>
    /// 文章修改日期
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime? UpdateDate { get; set; }

    /// <summary>
    /// 文章删除日期
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime? RemoveDate { get; set; }

    [InverseProperty("Article")]
    public virtual ICollection<ArticleArchive> ArticleArchives { get; set; } = new List<ArticleArchive>();

    [InverseProperty("Article")]
    public virtual ICollection<ArticleCategory> ArticleCategories { get; set; } = new List<ArticleCategory>();

    [InverseProperty("Article")]
    public virtual ICollection<ArticleComment> ArticleComments { get; set; } = new List<ArticleComment>();

    [InverseProperty("Article")]
    public ArticleContent ArticleContent { get; set; } = null!;

    [InverseProperty("Article")]
    public virtual ICollection<ArticleTag> ArticleTags { get; set; } = new List<ArticleTag>();
}
