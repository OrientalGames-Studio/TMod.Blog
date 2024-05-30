using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TMod.Blog.Data.Models;

/// <summary>
/// 应用配置表，可以用来存储一些应用设置，例如全局控制是否可以评论等
/// </summary>
[Index("Key", Name = "Uq__Configurations__Key", IsUnique = true)]
public partial class Configuration
{
    /// <summary>
    /// 自增主键
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// 配置项键
    /// </summary>
    [StringLength(50)]
    public string Key { get; set; } = null!;

    /// <summary>
    /// 配置项值
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// 是否已删除
    /// </summary>
    public bool IsRemove { get; set; }

    /// <summary>
    /// 配置项版本
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
}
