using System.Windows;
using Unity2Debug.Navigation;

namespace Unity2Debug
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var nav = new NavigationService(MainFrame);
            nav.NavigateTo(new Pages.InitialSetup());
        }
    }
}