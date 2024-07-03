using FluentValidation;
using Unity2Debug.Common.Utility.Tools;

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
                .Must(x => UnityTools.TryGetVaildUnityPath(out _, x.UnityInstallPath, x.RetailGameExe))
                .WithMessage(x => $"Unity Path Directory is invalid! Unity version: {UnityTools.GetUnityVersionFromAssembly(x.RetailGameExe)} install not found: Ensure proper version is installed and path points to either the Unity Hub directory (e.g. C:\\Program Files\\Unity\\Hub\\Editor) or the direct base path of the correct version. (e.g. C:\\Program Files\\Unity <version>)");

            RuleFor(x => x.UseSymlinks)
                .Equal(true)
                .WithSeverity(Severity.Warning)
                .WithMessage("It is highly recommended to use symlinks as it will greatly reduce space used and time to copy!");

            RuleFor(x => x.VerboseLogging)
                .Equal(false)
                .WithSeverity(Severity.Warning)
                .WithMessage("Verbose logging will slow down the copy process!");
        }
    }
}
