using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Domain.Interfaces.Repositories;

namespace TMod.Blog.Domain.Interfaces.UnitOfWorks
{
    public interface IArtifactUnitOfWork:IUnitOfWork
    {
        IArticleRepository ArticleRepository { get; }

        ICommentRepository CommentRepository { get; }

        IFavoriteRepository FavoriteRepository { get; }

        IShareLinkRepository ShareLinkRepository { get; }
    }
}
