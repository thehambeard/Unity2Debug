using System.Windows;
using System.Windows.Controls;
using Unity2Debug.Dialogs.ViewModel;

namespace Unity2Debug.Dialogs
{
    /// <summary>
    /// Interaction logic for LoadDefaults.xaml
    /// </summary>
    public partial class LoadDefaults : UserControl
    {
        private readonly LoadDefaultsVM _vm;
        public LoadDefaults()
        {
            InitializeComponent();
            _vm = new();
            DataContext = _vm;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Parent is Window window)
                window.DialogResult = true;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _vm.Logger = new(TextBoxLogger);
            _vm.Load();
        }
    }
}
