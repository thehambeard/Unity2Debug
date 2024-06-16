using FluentValidation;

namespace Unity2Debug.Common.SettingsService.Validators
{
    public class DebugSettingsValidator : ValidatorBase<DebugSettings>
    {
        public DebugSettingsValidator()
        {
            RuleFor(x => x.RetailGameExe)
                .Must(File.Exists)
                .WithMessage("{PropertyName} '{PropertyValue}' does not exist!");

            RuleFor(x => x.SteamAppId)
                .NotEmpty().WithSeverity(Severity.Warning)
                .WithMessage("A Steam App Id should be provided as Steamworks can interfere with debugging (GOG games sometimes have Steamworks if the game is on Steam as well). Find button will search games to get it automatically.");

            RuleFor(x => x.DebugOutputPath)
                .Cascade(CascadeMode.Stop)
                .Must(Directory.Exists)
                .WithMessage("{PropertyName} '{PropertyValue}' does not exist!")
                .Must(DirectoryDoesNotContainData).WithSeverity(Severity.Warning)
                .WithMessage("{PropertyName} '{PropertyValue}' contains data, this is not recommended");

            RuleFor(x => x)
                .Cascade(CascadeMode.Stop)
                .Must(x => Directory.Exists(x.UnityInstallPath))
                .WithMessage(x => $"Unity Path Directory '{x.UnityInstallPath}' does not exist!")
                .Must(x => x.UnityVersions.Count > 0)
                .WithMessage(x => $"Unity Path Directory {x.UnityInstallPath} does not contain Unity versions. Ensure directory is set to a hub directory (e.g. C:\\Program Files\\Unity\\Hub\\Editor)")
                .Must(x => Directory.Exists(Path.Combine(x.UnityInstallPath, x.UnityVersion)))
                .WithMessage(x => $"Unity Version '{x.UnityVersion}' directory does not exist, please check version is installed");

            RuleFor(x => x.UseSymlinks)
                .Equal(true)
                .WithSeverity(Severity.Warning)
                .WithMessage("It is highly recommended to use symlinks as it will greatly reduce space used and time to copy!");
        }
    }
}
