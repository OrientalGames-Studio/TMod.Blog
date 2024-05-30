using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

using TMod.Blog.Data.Interfaces;

namespace TMod.Blog.Data.Models;

/// <summary>
/// 博客文章的评论表，用于存储访客对文章的评论或对评论的回复
/// </summary>
public partial class ArticleComment:IGuidKey,IVersionControl,ICreate,IUpdate,IRemove,IEdit,INotifiable,IKey<Guid>
{
    /// <summary>
    /// 评论主键标识
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// 文章外键标识
    /// </summary>
    public Guid ArticleId { get; set; }

    /// <summary>
    /// 父级评论标识
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// 评论内容
    /// </summary>
    public string Content { get; set; } = null!;

    /// <summary>
    /// 评论内容的二进制数据
    /// </summary>
    public byte[] ByteContent { get; set; } = null!;

    /// <summary>
    /// 评论状态（0:正常展示，1：评论不宜展示）
    /// </summary>
    public short State { get; set; }

    /// <summary>
    /// 是否在收到回复时发送通知
    /// </summary>
    public bool NotifitionWhenReply { get; set; }

    /// <summary>
    /// 是否已删除
    /// </summary>
    public bool IsRemove { get; set; }

    /// <summary>
    /// 评论版本
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// 评论者昵称
    /// </summary>
    [StringLength(50)]
    public string CreateUserName { get; set; } = null!;

    /// <summary>
    /// 评论者邮箱，用于通知有新回复
    /// </summary>
    [StringLength(20)]
    public string CreateUserEmail { get; set; } = null!;

    /// <summary>
    /// 评论发表时间
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    /// <summary>
    /// 修改时间
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime? UpdateDate { get; set; }

    /// <summary>
    /// 上次编辑评论时间
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime? LastEditDate { get; set; }

    /// <summary>
    /// 删除时间
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime? RemoveDate { get; set; }

    [ForeignKey("ArticleId")]
    [InverseProperty("ArticleComments")]
    public virtual Article Article { get; set; } = null!;
}
