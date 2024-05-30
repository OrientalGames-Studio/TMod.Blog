using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TMod.Blog.Data.Models;

namespace TMod.Blog.Data;

public partial class BlogContext : DbContext
{
    public BlogContext(DbContextOptions<BlogContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Article> Articles { get; set; }

    public virtual DbSet<ArticleArchive> ArticleArchives { get; set; }

    public virtual DbSet<ArticleCategory> ArticleCategories { get; set; }

    public virtual DbSet<ArticleComment> ArticleComments { get; set; }

    public virtual DbSet<ArticleContent> ArticleContents { get; set; }

    public virtual DbSet<ArticleTag> ArticleTags { get; set; }

    public virtual DbSet<ArticleTagsView> ArticleTagsViews { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Configuration> Configurations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Pk__Articles_Id");

            entity.ToTable(tb => tb.HasComment("博客文章主表，用来存储博客的文章快照（标题、状态、简讯等）"));

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasComment("Uid 主键标识");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasComment("文章创建日期");
            entity.Property(e => e.IsCommentEnabled).HasComment("文章是否允许评论");
            entity.Property(e => e.IsRemove).HasComment("是否已删除");
            entity.Property(e => e.LastEditDate).HasComment("最后一次编辑文章日期");
            entity.Property(e => e.PublishDate).HasComment("文章首次发表日期");
            entity.Property(e => e.RemoveDate).HasComment("文章删除日期");
            entity.Property(e => e.Snapshot).HasComment("文章前150个字内容节选");
            entity.Property(e => e.State).HasComment("文章状态（0：草稿，1：已发布，2：已隐藏）");
            entity.Property(e => e.Title).HasComment("文章标题");
            entity.Property(e => e.UpdateDate).HasComment("文章修改日期");
            entity.Property(e => e.Version).HasComment("文章版本");
        });

        modelBuilder.Entity<ArticleArchive>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Pk__ArticleArchives__Id");

            entity.ToTable(tb => tb.HasComment("博客的文章附件表，用来存储博客文章的附件"));

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasComment("附件主键标识");
            entity.Property(e => e.ArchiveContent).HasComment("附件文件数据");
            entity.Property(e => e.ArchiveFileSize).HasComment("文件大小");
            entity.Property(e => e.ArchiveMimetype)
                .HasDefaultValue("application/octet-stream")
                .HasComment("附件MIME类型");
            entity.Property(e => e.ArchiveName)
                .HasDefaultValueSql("(newid())")
                .HasComment("文件名称");
            entity.Property(e => e.ArticleId).HasComment("文章外键标识");

            entity.HasOne(d => d.Article).WithMany(p => p.ArticleArchives)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Fk__ArticleArchives_ArticleId");
        });

        modelBuilder.Entity<ArticleCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Pk__ArticleCategories__Id");

            entity.ToTable(tb => tb.HasComment("博客的文章内容表，用来存储博客的正文内容"));

            entity.Property(e => e.Id).HasComment("自增主键");
            entity.Property(e => e.ArticleId).HasComment("文章外键标识");
            entity.Property(e => e.CategoryId).HasComment("分类外键标识");

            entity.HasOne(d => d.Article).WithMany(p => p.ArticleCategories).HasConstraintName("Fk__ArticleCategories_ArticleId");

            entity.HasOne(d => d.Category).WithMany(p => p.ArticleCategories).HasConstraintName("Fk__ArticleCategories_CategoryId");
        });

        modelBuilder.Entity<ArticleComment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Pk__ArticleComments__Id");

            entity.ToTable(tb => tb.HasComment("博客文章的评论表，用于存储访客对文章的评论或对评论的回复"));

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasComment("评论主键标识");
            entity.Property(e => e.ArticleId).HasComment("文章外键标识");
            entity.Property(e => e.ByteContent).HasComment("评论内容的二进制数据");
            entity.Property(e => e.Content).HasComment("评论内容");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasComment("评论发表时间");
            entity.Property(e => e.CreateUserEmail).HasComment("评论者邮箱，用于通知有新回复");
            entity.Property(e => e.CreateUserName)
                .HasDefaultValue("陌生网友")
                .HasComment("评论者昵称");
            entity.Property(e => e.IsRemove).HasComment("是否已删除");
            entity.Property(e => e.LastEditDate).HasComment("上次编辑评论时间");
            entity.Property(e => e.NotifitionWhenReply).HasComment("是否在收到回复时发送通知");
            entity.Property(e => e.ParentId).HasComment("父级评论标识");
            entity.Property(e => e.RemoveDate).HasComment("删除时间");
            entity.Property(e => e.State).HasComment("评论状态（0:正常展示，1：评论不宜展示）");
            entity.Property(e => e.UpdateDate).HasComment("修改时间");
            entity.Property(e => e.Version).HasComment("评论版本");

            entity.HasOne(d => d.Article).WithMany(p => p.ArticleComments).HasConstraintName("Fk__ArticleComments_ArticleId");
        });

        modelBuilder.Entity<ArticleContent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Pk__ArticleContent_Id");

            entity.ToTable(tb => tb.HasComment("博客的文章内容表，用来存储博客的正文内容"));

            entity.Property(e => e.Id).HasComment("自增主键");
            entity.Property(e => e.ArticleId).HasComment("文章编号外键");
            entity.Property(e => e.Content).HasComment("文章正文内容");

            entity.HasOne(d => d.Article).WithMany(p => p.ArticleContents).HasConstraintName("Fk__Article_Id");
        });

        modelBuilder.Entity<ArticleTag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Pk__ArticleTags__Id");

            entity.ToTable(tb => tb.HasComment("博客的文章标签表，用来存储博客的标签"));

            entity.Property(e => e.Id).HasComment("自增主键");
            entity.Property(e => e.ArticleId).HasComment("文章外键标识");
            entity.Property(e => e.Tag).HasComment("文章标签");

            entity.HasOne(d => d.Article).WithMany(p => p.ArticleTags).HasConstraintName("Fk__ArticleTags_ArticleId");
        });

        modelBuilder.Entity<ArticleTagsView>(entity =>
        {
            entity.ToView("ArticleTagsView");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Pk__Categories__Id");

            entity.ToTable(tb => tb.HasComment("博客的文章分类表，用来存储博客可以选择的分类"));

            entity.Property(e => e.Id).HasComment("自增主键");
            entity.Property(e => e.Category1).HasComment("分类标签");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasComment("创建日期");
            entity.Property(e => e.IsRemove).HasComment("是否已删除");
            entity.Property(e => e.RemoveDate).HasComment("删除日期");
            entity.Property(e => e.UpdateDate).HasComment("修改日期");
            entity.Property(e => e.Version).HasComment("分类版本");
        });

        modelBuilder.Entity<Configuration>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Pk__Configurations__Id");

            entity.ToTable(tb => tb.HasComment("应用配置表，可以用来存储一些应用设置，例如全局控制是否可以评论等"));

            entity.Property(e => e.Id).HasComment("自增主键");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasComment("创建日期");
            entity.Property(e => e.IsRemove).HasComment("是否已删除");
            entity.Property(e => e.Key).HasComment("配置项键");
            entity.Property(e => e.RemoveDate).HasComment("删除日期");
            entity.Property(e => e.UpdateDate).HasComment("修改日期");
            entity.Property(e => e.Value).HasComment("配置项值");
            entity.Property(e => e.Version).HasComment("配置项版本");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
