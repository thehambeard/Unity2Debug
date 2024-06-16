using ICSharpCode.Decompiler;
using System.Windows;
using System.Windows.Controls;
using Unity2Debug.Settings;

namespace Unity2Debug.Pages
{
    /// <summary>
    /// Interaction logic for Processing.xaml
    /// </summary>
    public partial class Processing : Page
    {
        private readonly ProcessingVM _vm;

        private string? currentTitle;

        public Processing(ObservableProfiles profiles)
        {
            InitializeComponent();

            var progress = new Progress<DecompilationProgress>();

            progress.ProgressChanged += (_, value) =>
            {
                if (currentTitle != value.Title)
                    ProgressBar.Value = 0;

                var percent = value.UnitsCompleted * 100 / value.TotalUnits;

                ProgressBar.Value = Math.Max(percent, ProgressBar.Value);
            };

            _vm = new ProcessingVM(new(RichTextBoxLogger), profiles, progress);
            DataContext = _vm;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _vm.StartCommand.ExecuteAsync(null);
        }
    }
}
