using System.Windows;
using Unity2Debug.Dialogs;
using Unity2Debug.Dialogs.ViewModel;

namespace Unity2Debug
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            DialogService.DialogService.RegisterDialog<SymlinkDialogVM, SymlinkDialog>();
        }
    }
}
