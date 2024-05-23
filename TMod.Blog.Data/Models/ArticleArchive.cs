using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

using TMod.Blog.Data.Interfaces;

namespace TMod.Blog.Data.Models;

/// <summary>
/// 博客的文章附件表，用来存储博客文章的附件
/// </summary>
public partial class ArticleArchive: IGuidKey, IKey<Guid>
{
    /// <summary>
    /// 附件主键标识
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// 文章外键标识
    /// </summary>
    public Guid ArticleId { get; set; }

    /// <summary>
    /// 附件文件数据
    /// </summary>
    public byte[] ArchiveContent { get; set; } = null!;

    [ForeignKey("ArticleId")]
    [InverseProperty("ArticleArchives")]
    public virtual Article Article { get; set; } = null!;
}
