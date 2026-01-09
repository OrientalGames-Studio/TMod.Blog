using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Domain.Interfaces.Repositories;
using TMod.Blog.Domain.Interfaces.UnitOfWorks;
using TMod.Blog.Infrastructure.Contextes;

namespace TMod.Blog.Infrastructure.Repositories.UnitOfWorks
{
    internal class ApplicationUnitOfWork : UnitOfWork, IApplicationUnitOfWork
    {

        public IArticleRepository ArticleRepository { get; set; }

        public ICategoryRepository CategoryRepository { get; set; }

        public ICommentRepository CommentRepository { get; set; }

        public IShareLinkRepository ShareLinkRepository { get; set; }

        public ISiteConfigurationRepository SiteConfigurationRepository { get; set; }

        public ITagRepository TagRepository { get; set; }

        public IFavoriteRepository FavoriteRepository { get; set; }

        public ApplicationUnitOfWork(TMod_Blog_RW_Context _context
            ,IArticleRepository articleRepository
            ,ICategoryRepository categoryRepository
            ,ICommentRepository commentRepository
            ,IShareLinkRepository shareLinkRepository
            ,ISiteConfigurationRepository siteConfigurationRepository
            ,ITagRepository tagRepository
            ,IFavoriteRepository favoriteRepository) : base(_context)
        {
            ArticleRepository = articleRepository;
            CategoryRepository = categoryRepository;
            CommentRepository = commentRepository;
            ShareLinkRepository = shareLinkRepository;
            SiteConfigurationRepository = siteConfigurationRepository;
            TagRepository = tagRepository;
            FavoriteRepository = favoriteRepository;
        }
    }
}
