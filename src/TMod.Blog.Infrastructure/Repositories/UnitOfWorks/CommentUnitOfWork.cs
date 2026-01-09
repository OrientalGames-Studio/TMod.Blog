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
    internal class CommentUnitOfWork : UnitOfWork, ICommentUnitOfWork
    {
        public IArticleRepository ArticleRepository { get; set; }

        public ICommentRepository CommentRepository { get; set; }

        public IFavoriteRepository FavoriteRepository { get; set; }
        public CommentUnitOfWork(TMod_Blog_RW_Context _context,IArticleRepository articleRepository,ICommentRepository commentRepository,IFavoriteRepository favoriteRepository) : base(_context)
        {
            ArticleRepository = articleRepository;
            CommentRepository = commentRepository;
            FavoriteRepository = favoriteRepository;
        }
    }
}
