using FluentValidation;

namespace Unity2Debug.Common.SettingsService.Validators
{
    public class DecompileSettingsValidator : ValidatorBase<DecompileSettings>
    {
        public DecompileSettingsValidator()
        {
            RuleFor(x => x.OutputDirectory)
                .Cascade(CascadeMode.Stop)
                .Must(Directory.Exists).WithMessage("{PropertyName} '{PropertyValue}' does not exist!")
                .Must(DirectoryDoesNotContainData).WithSeverity(Severity.Warning).WithMessage("{PropertyName} '{PropertyValue}' contains data! This is not recommended.");

            RuleFor(x => x.AssemblyPaths)
                .NotEmpty().WithMessage("{PropertyName} must not be empty.")
                .ForEach(x =>
                {
                    x.NotEmpty()
                    .Must(File.Exists).WithMessage("{PropertyName} contains file '{PropertyValue}' that does not exist");
                });
        }
    }
}
