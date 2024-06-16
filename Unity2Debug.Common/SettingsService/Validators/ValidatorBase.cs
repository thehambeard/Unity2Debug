using FluentValidation;
namespace Unity2Debug.Common.SettingsService.Validators
{
    public class ValidatorBase<T> : AbstractValidator<T>
    {
        protected bool DirectoryDoesNotContainData(string path) => !(string.IsNullOrEmpty(path) || !Directory.Exists(path) || Directory.GetDirectories(path).Length != 0 || Directory.GetFiles(path).Length != 0);
    }
}
