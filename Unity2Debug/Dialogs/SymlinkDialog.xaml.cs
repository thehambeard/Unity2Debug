using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Unity2Debug.Dialogs.ViewModel;

namespace Unity2Debug.Dialogs
{
    /// <summary>
    /// Interaction logic for SymlinkDialog.xaml
    /// </summary>
    public partial class SymlinkDialog : UserControl
    {
        private readonly SymlinkDialogVM _vm;
        public SymlinkDialog()
        {
            InitializeComponent();
            _vm = new();
            DataContext = _vm;
        }
        private void ListBox_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = ItemsControl.ContainerFromElement(sender as ListBox, e.OriginalSource as DependencyObject) as ListBoxItem;
            if (item != null)
                FilterTextBox.Text = item.DataContext as string;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Parent is Window window)
                window.DialogResult = true;
        }
    }
}
