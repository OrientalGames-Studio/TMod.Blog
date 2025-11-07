using FluentValidation;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Application.Requests.Comment;

namespace TMod.Blog.Application.Validators.Comment
{
    internal class CreateCommentRequestValidator:AbstractValidator<CreateCommentRequest>
    {
        public CreateCommentRequestValidator()
        {
            RuleFor(c => c.AuthorName)
                .NotEmpty().WithMessage("请输入你的昵称")
                .MaximumLength(64).WithMessage("昵称过长");

            RuleFor(c => c.AuthorEmail)
                .NotEmpty().WithMessage("请输入邮箱，以便于接收回复提醒")
                .MaximumLength(128).WithMessage("邮箱太长了")
                .EmailAddress(FluentValidation.Validators.EmailValidationMode.AspNetCoreCompatible).WithMessage("请输入正确的邮箱");

            RuleFor(c => c.Content)
                .NotEmpty().WithMessage("不允许回复空白内容");

            RuleFor(c => c.ArticleId)
                .NotNull()
                .NotEqual(Guid.Empty)
                .WithMessage("必须评论给一个文章");
        }
    }
}
