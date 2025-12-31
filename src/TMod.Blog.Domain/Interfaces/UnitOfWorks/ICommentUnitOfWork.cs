using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Domain.Interfaces.Repositories;

namespace TMod.Blog.Domain.Interfaces.UnitOfWorks
{
    public interface ICommentUnitOfWork:IUnitOfWork
    {
        IArticleRepository ArticleRepository { get; }

        ICommentRepository CommentRepository { get; }
    }
}
