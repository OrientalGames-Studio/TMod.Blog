using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Domain.Entities;

namespace TMod.Blog.Infrastructure.Contextes
{
    public class TMod_Blog_RW_Context : DbContext
    {

        public DbSet<Article> Articles => Set<Article>();

        public DbSet<Category> Categories => Set<Category>();

        public DbSet<Tag> Tags => Set<Tag>();

        public DbSet<Comment> Comments => Set<Comment>();

        public DbSet<ShareLink> ShareLinks => Set<ShareLink>();

        public DbSet<SiteConfiguration> SiteConfigurations => Set<SiteConfiguration>();

        public TMod_Blog_RW_Context(DbContextOptions options) : base(options)
        {
        }

        protected TMod_Blog_RW_Context()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            ConfigureCategory(modelBuilder);
            ConfigureArticle(modelBuilder);
            ConfigureTag(modelBuilder);
            ConfigureComment(modelBuilder);
            ConfigureShareLink(modelBuilder);
            ConfigureSiteConfiguration(modelBuilder);
            SeedInitialData(modelBuilder);
        }

        private void SeedInitialData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
                .HasData(new Category()
                {
                    Id = Guid.Parse("3B85920F-76BE-4AD2-AD96-FD95B9BFB2DC"),
                    Name = "未分类",
                    Description = "没有分类或暂无分类的文章",
                    ParentId = null,
                    CreateDate = DateTime.Parse("2025-11-04 10:41"),
                    IsDeleted = false,
                });

            modelBuilder.Entity<SiteConfiguration>()
                .HasData(
                    new SiteConfiguration()
                    {
                        Id = 1,
                        ConfigKey = "SITE_TITLE",
                        ConfigValue = "我的个人博客",
                        Description = "站点标题配置项",
                        CreateDate = DateTime.Parse("2025-11-04 10:41")
                    },
                    new SiteConfiguration()
                    {
                        Id = 2,
                        ConfigKey = "SITE_SHORT_CODE_WORKER_ID",
                        ConfigValue = "522",
                        Description = "短码服务的 Worker Id",
                        CreateDate = DateTime.Parse("2025-11-07 17:40")
                    },
                    new SiteConfiguration()
                    {
                        Id = 3,
                        ConfigKey = "SITE_SHORT_CODE_EPOCH",
                        ConfigValue = "2025-01-01",
                        Description = "短码服务的 epoch",
                        CreateDate = DateTime.Parse("2025-11-07 17:40")
                    },
                    new SiteConfiguration()
                    {
                        Id = 4,
                        ConfigKey = "SITE_SHORT_CODE_SECRET_KEY",
                        ConfigValue = "ZyfPassw0rd!Blog",
                        Description = "短码服务的密钥",
                        CreateDate = DateTime.Parse("2025-11-07 17:40")
                    },
                    new SiteConfiguration()
                    {
                        Id = 5,
                        ConfigKey = "SITE_SHORT_CODE_MIN_LENGTH",
                        ConfigValue = "6",
                        Description = "短码服务生成的短码最少几位",
                        CreateDate = DateTime.Parse("2025-11-07 17:40")
                    },
                    new SiteConfiguration()
                    {
                        Id = 6,
                        ConfigKey = "SITE_SHORT_CODE_MAX_LENGTH",
                        ConfigValue = "20",
                        Description = "短码服务生成的短码最多几位",
                        CreateDate = DateTime.Parse("2025-11-07 17:40")
                    }
                );
        }

        private void ConfigureSiteConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SiteConfiguration>(entity =>
            {
                entity.HasIndex(c => c.ConfigKey);
                entity.ToTable("system_configurations");
            });
        }

        private void ConfigureShareLink(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShareLink>()
                .ToTable("share_links");
        }

        private void ConfigureComment(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.Property(c => c.Id)
                .HasDefaultValueSql<Guid>("NEWSEQUENTIALID()");

                entity.HasKey(entity=>entity.Id);

                entity.HasOne(c=>c.Article)
                .WithMany(a=>a.Comments)
                .HasForeignKey(c=>c.ArticleId)
                .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c=>c.Parent)
                .WithMany(c=>c.Replies)
                .HasForeignKey(c=>c.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.ToTable("comments");
            });
        }

        private void ConfigureTag(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tag>(entity =>
            {
                entity.Property(t => t.Id)
                .HasDefaultValueSql<Guid>("NEWSEQUENTIALID()");

                entity.ToTable("tags");
            });
        }

        private void ConfigureArticle(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>(entity =>
            {
                entity.ToTable("articles");

                entity.Property(a => a.Id)
                .HasDefaultValueSql<Guid>("NEWSEQUENTIALID()");

                entity.HasIndex(a => a.Slug)
                .IsUnique();

                entity.Property(a => a.IsShareEnabled)
                .HasDefaultValue(true);

                entity.Property(a=>a.IsCommentEnabled)
                .HasDefaultValue(true);

                entity.HasMany(a => a.Tags)
                .WithMany(t => t.Articles)
                .UsingEntity<Dictionary<string, object>>(
                    "ArticleTag"
                    , j => j.HasOne<Tag>()
                    .WithMany()
                    .HasForeignKey("TagId")
                    .OnDelete(DeleteBehavior.Cascade)
                    , j => j.HasOne<Article>()
                    .WithMany()
                    .HasForeignKey("ArticleId")
                    .OnDelete(DeleteBehavior.Cascade)
                    , j =>
                    {
                        j.HasKey("TagId", "ArticleId");
                        j.ToTable("article_tags");
                    });
            });
        }

        private void ConfigureCategory(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(c => c.Id)
                .HasDefaultValueSql<Guid>("NEWSEQUENTIALID()");

                entity.HasOne(c=>c.Parent)
                .WithMany(c=>c.Children)
                .HasForeignKey(c=>c.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(c => c.Articles)
                .WithOne(a => a.Category)
                .HasForeignKey(a => a.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

                entity.ToTable("categories");
            });
        }
    }
}
