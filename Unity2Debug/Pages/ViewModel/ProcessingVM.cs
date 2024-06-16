using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ICSharpCode.Decompiler;
using Unity2Debug.Common.Automation;
using Unity2Debug.Logging;
using Unity2Debug.Settings;

namespace Unity2Debug.Pages
{
    public partial class ProcessingVM : ObservableObject
    {
        private readonly Automator _automator;
        private readonly CancellationTokenSource _cancellationTokenSource;
        public ProcessingVM(RichTextBoxLogger logger, ObservableProfiles profiles, IProgress<DecompilationProgress> progress)
        {
            if (profiles.CurrentProfile == null)
                throw new NullReferenceException();

            _cancellationTokenSource = new CancellationTokenSource();

            _automator = new(
                logger,
                profiles.CurrentProfile.DecompileSettings.ToNonObservableSettings(),
                profiles.CurrentProfile.DebugSettings.ToNonObservableSettings(),
                _cancellationTokenSource.Token,
                progress);
        }

        [RelayCommand]
        public async Task StartAsync()
        {
            await Task.Run(_automator.Start);
        }
    }
}
