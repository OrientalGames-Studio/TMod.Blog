using FluentValidation;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Application.Requests.Category;

namespace TMod.Blog.Application.Validators.Category
{
    internal class PatchCategoryParentRequestValidator:AbstractValidator<PatchCategoryParentRequest>
    {
        public PatchCategoryParentRequestValidator()
        {
            RuleFor(c => c.ParentId)
                .NotNull()
                .NotEqual(Guid.Empty)
                .WithMessage("请选择一个父级分类");
        }
    }
}
