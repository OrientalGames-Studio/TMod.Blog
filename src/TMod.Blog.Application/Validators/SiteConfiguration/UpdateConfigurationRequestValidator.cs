using FluentValidation;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Application.Requests.SiteConfiguration;

namespace TMod.Blog.Application.Validators.SiteConfiguration
{
    internal class UpdateConfigurationRequestValidator : AbstractValidator<UpdateConfigurationRequest>
    {
        public UpdateConfigurationRequestValidator()
        {
            RuleFor(c => c.ConfigKey)
                .NotEmpty().WithMessage("配置项不允许为空");
        }
    }
}
