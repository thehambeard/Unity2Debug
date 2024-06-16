using FluentValidation.Results;
using Newtonsoft.Json;
using Unity2Debug.Common.SettingsService.Validators;

namespace Unity2Debug.Common.SettingsService
{
    public abstract class SettingsBase<TSettings>
    {
        protected ValidatorBase<TSettings>? _validator;

        [JsonIgnore]
        public abstract bool HasErrors { get; }

        public void RegisterValidator<TValidator>(TValidator validator) where TValidator : ValidatorBase<TSettings> =>
            _validator = validator;

        public ValidatorBase<TSettings>? GetValidator() => _validator;
    }

    public class SettingsBase<TSettings, TValidator> : SettingsBase<TSettings>
        where TSettings : class
        where TValidator : ValidatorBase<TSettings>, new()
    {
        public override bool HasErrors => this.Validate().Errors.Any(x => x.Severity == FluentValidation.Severity.Error);

        public SettingsBase()
        {
            RegisterValidator<TValidator>(new());
        }
    }

    public static class SettingsExtensions
    {
        public static ValidationResult Validate<TSettings>(this SettingsBase<TSettings> setting)
            where TSettings : class
        {
            ValidationResult result = new();

            var validator = setting.GetValidator();

            if (validator != null && setting is TSettings settings)
                result = validator.Validate(settings);

            return result;
        }
    }
}
