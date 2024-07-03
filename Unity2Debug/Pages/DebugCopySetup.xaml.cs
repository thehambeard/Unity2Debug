using System.Windows.Controls;
using Unity2Debug.Pages.ViewModel;
using Unity2Debug.Settings;

namespace Unity2Debug.Pages
{
    /// <summary>
    /// Interaction logic for DebugCopySetup.xaml
    /// </summary>
    public partial class DebugCopySetup : Page
    {
        private readonly DebugCopySetupVM _vm;
        public DebugCopySetup(ObservableProfiles profiles)
        {
            InitializeComponent();

            _vm = new DebugCopySetupVM(new(TextBoxLogger), new DialogService.DialogService(), profiles);
            DataContext = _vm;
        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
        }
    }
}
