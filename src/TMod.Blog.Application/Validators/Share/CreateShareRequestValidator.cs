using FluentValidation;

using System;
using System.Collections.Generic;
using System.Text;

using TMod.Blog.Application.Common.Enums;
using TMod.Blog.Application.Requests.Share;

namespace TMod.Blog.Application.Validators.Share
{
    internal class CreateShareRequestValidator:AbstractValidator<CreateShareRequest>
    {
        public CreateShareRequestValidator()
        {
            RuleFor(x => x.ExpireAt)
                .GreaterThan(0)
                .When(x => x.AutoExpire)
                .WithMessage("自动过期时，ExpireAt 必须大于 0");

            RuleFor(x => x.DaysUnit)
                .IsInEnum()
                .When(x=>x.AutoExpire)
                .WithMessage("日期单位必须是有效的 DaysUnit 值");

            RuleFor(x => x)
                .Custom((model, context) =>
                {
                    if ( !model.AutoExpire )
                    {
                        return;
                    }
                    if ( !Enum.IsDefined(typeof(DaysUnitEnum), model.DaysUnit) )
                    {
                        return;
                    }
                    if(model.ExpireAt <= 0 )
                    {
                        context.AddFailure(nameof(model.ExpireAt), "自动过期时间最小应该大于1天");
                        return;
                    }
                    var totaldays = ToDays(model.ExpireAt,model.DaysUnit);
                    if(totaldays > 365 )
                    {
                        context.AddFailure(nameof(model.ExpireAt), "自动过期时间最大不能超过1年");
                    }
                })
                .When(x=>x.AutoExpire);
        }

        private int ToDays(int expireAt,DaysUnitEnum daysUnit)
        {
            return daysUnit switch
            {
                DaysUnitEnum.Days => expireAt,
                DaysUnitEnum.Weeks => expireAt * 7,
                DaysUnitEnum.Months => expireAt * 30,
                DaysUnitEnum.Seasons => expireAt * 90,
                DaysUnitEnum.Years => expireAt * 365,
                _ => throw new ArgumentOutOfRangeException(nameof(daysUnit), daysUnit, "日期单位不是有效的 DaysUnit 值")
            };
        }
    }
}
