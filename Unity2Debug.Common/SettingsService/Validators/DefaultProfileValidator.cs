using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unity2Debug.Common.SettingsService.Validators
{
    internal class DefaultProfileValidator : ValidatorBase<DefaultProfile>
    {
        public DefaultProfileValidator(List<string> profileNames, Func<string>? checkExeFallback) 
        {
            RuleFor(x => x)
                .Cascade(CascadeMode.Stop)
                .Must((x) => !string.IsNullOrEmpty(x.Name))
                .WithMessage("Default profile name must not be empty or null")
                .Must((x) => !profileNames.Contains(x.Name))
                .WithMessage((x) => $"Profile with name: {x.Name} already exists")
                .Must(x => x.CheckExePath(checkExeFallback))
                .WithMessage(x => $"{x.Name} install not found. If game is installed ensure the retail game is run at least once (Kingmaker, WOTR, RT only).");
        }
    }
}
