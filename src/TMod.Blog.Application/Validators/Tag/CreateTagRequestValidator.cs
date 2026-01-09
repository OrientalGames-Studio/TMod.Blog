using FluentValidation;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Application.Requests.Tag;

namespace TMod.Blog.Application.Validators.Tag
{
    internal class CreateTagRequestValidator : AbstractValidator<CreateTagRequest>
    {
        public CreateTagRequestValidator()
        {
            RuleFor(t => t.Name)
                .NotEmpty().WithMessage("标签名称不能为空")
                .MaximumLength(64).WithMessage("标签名称过长");
        }
    }
}
