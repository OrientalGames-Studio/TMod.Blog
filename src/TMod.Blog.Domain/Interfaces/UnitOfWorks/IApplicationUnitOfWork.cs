using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Domain.Interfaces.Repositories;

namespace TMod.Blog.Domain.Interfaces.UnitOfWorks
{
    /// <summary>
    /// 应用级别的工作单元接口
    /// </summary>
    public interface IApplicationUnitOfWork:IUnitOfWork
    {
        IArticleRepository ArticleRepository { get; }

        ICategoryRepository CategoryRepository { get; }

        ICommentRepository CommentRepository { get; }

        IShareLinkRepository ShareLinkRepository { get; }

        ISiteConfigurationRepository SiteConfigurationRepository { get; }

        ITagRepository TagRepository { get; }

        IFavoriteRepository FavoriteRepository { get; }
    }
}
