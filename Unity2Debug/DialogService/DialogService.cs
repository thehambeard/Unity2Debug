using Ookii.Dialogs.Wpf;
using System.Windows;
using Unity2Debug.Dialogs;

namespace Unity2Debug.DialogService
{
    internal class DialogService : IDialogService
    {
        private static Dictionary<Type, Type> _mappings = [];

        public static void RegisterDialog<TViewModel, TView>()
        {
            if (!_mappings.ContainsKey(typeof(TViewModel)))
                _mappings.Add(typeof(TViewModel), typeof(TView));
        }

        public string? OpenFile(string filter)
        {
            var openFileDialog = new VistaOpenFileDialog();
            openFileDialog.Filter = filter;

            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                return openFileDialog.FileName;
            }
            return null;
        }

        public List<string> OpenFiles(string filter)
        {
            var openFileDialog = new VistaOpenFileDialog();
            openFileDialog.Filter = filter;
            openFileDialog.Multiselect = true;

            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                return [.. openFileDialog.FileNames];
            }
            return [];
        }

        public string? OpenFolder()
        {
            var folderBrowserDialog = new VistaFolderBrowserDialog();
            var result = folderBrowserDialog.ShowDialog();

            if (result == true)
                return folderBrowserDialog.SelectedPath;

            return null;
        }

        public void ShowDialog<TViewModel>(TViewModel? modelInstance, Action<bool?, TViewModel>? action = null)
            where TViewModel : class
        {
            if (!_mappings.ContainsKey(typeof(TViewModel)))
                throw new KeyNotFoundException($"Key: {nameof(TViewModel)} not found in the dialog register.");

            var dialog = new DialogWindow();
            var content = Activator.CreateInstance(_mappings[typeof(TViewModel)]);

            modelInstance ??= (TViewModel?)Activator.CreateInstance(typeof(TViewModel));

            void closeEventHandler(object? s, EventArgs e)
            {
                if (modelInstance is TViewModel viewModel)
                    action?.Invoke(dialog.DialogResult, viewModel);

                dialog.Closed -= closeEventHandler;
            }

            dialog.Closed += closeEventHandler;
            dialog.Content = content;

            if (content is FrameworkElement element)
                element.DataContext = modelInstance;

            dialog.ShowDialog();
        }
    }
}
