using FluentValidation;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Application.Requests.Article;

namespace TMod.Blog.Application.Validators.Article
{
    internal class UpdateArticleRequestValidator : AbstractValidator<UpdateArticleRequest>
    {
        public UpdateArticleRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("文章标题不允许为空")
                .MaximumLength(128)
                .WithMessage("文章标题长度不允许超过 128 字符");

            RuleFor(x => x.CategoryId)
                .NotNull()
                .NotEqual(Guid.Empty)
                .WithMessage("请选择一个分类");
        }
    }
}
