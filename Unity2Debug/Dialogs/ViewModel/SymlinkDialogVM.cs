using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.IO;
using Unity2Debug.Common.Utility;

namespace Unity2Debug.Dialogs.ViewModel
{
    public partial class SymlinkDialogVM : ViewModelBase
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Affected))]
        private string _basePath;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Affected))]
        private string _filter;

        [ObservableProperty]
        private ObservableCollection<string> _filters = [];

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Affected))]
        private string? _selectedFilter;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Affected))]
        private bool _showFiles;

        public SymlinkDialogVM()
        {
            BasePath = string.Empty;
            Filter = "*.*";
            ShowFiles = true;
        }

        [RelayCommand]
        private void AddFilter()
        {
            if (!string.IsNullOrEmpty(Filter) && Affected.Count > 0 && !Filters.Contains(Filter))
            {
                if (ShowFiles)
                    Filters.Add(Filter);
                else
                    Filters.Add(Filter.EnsureSeparator());
            }
        }

        [RelayCommand]
        private void RemoveFilter()
        {
            if (SelectedFilter != null && Filters.Contains(SelectedFilter))
                Filters.Remove(SelectedFilter);
        }

        public ObservableCollection<string> Affected
        {
            get
            {
                ObservableCollection<string> result = [];

                try
                {
                    if (ShowFiles)
                    {
                        foreach (var file in Directory.GetFiles(BasePath, Filter, SearchOption.AllDirectories))
                            result.Add(Path.GetRelativePath(BasePath, file));
                    }
                    else
                    {
                        foreach (var dir in Directory.GetDirectories(BasePath, Filter, SearchOption.AllDirectories))
                            result.Add(Path.GetRelativePath(BasePath, dir));
                    }
                }
                catch
                {
                }

                return result;
            }
        }
    }
}
