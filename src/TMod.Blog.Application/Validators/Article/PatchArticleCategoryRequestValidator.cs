using FluentValidation;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Application.Requests.Article;

namespace TMod.Blog.Application.Validators.Article
{
    internal class PatchArticleCategoryRequestValidator : AbstractValidator<PatchArticleCategoryRequest>
    {
        public PatchArticleCategoryRequestValidator()
        {
            RuleFor(x => x.CategoryId)
                .NotNull()
                .NotEqual(Guid.Empty)
                .WithMessage("请选择一个分类");
        }
    }
}
