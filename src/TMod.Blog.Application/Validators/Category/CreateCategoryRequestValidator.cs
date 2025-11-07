using FluentValidation;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Application.Requests.Category;

namespace TMod.Blog.Application.Validators.Category
{
    internal class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
    {
        public CreateCategoryRequestValidator()
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("分类名称不允许为空")
                .MaximumLength(64).WithMessage("分类名称过长");

            RuleFor(c => c.Description)
                .MaximumLength(200).WithMessage("描述信息过长");

        }
    }
}
