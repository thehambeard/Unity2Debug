namespace Unity2Debug.DialogService
{
    public interface IDialogService
    {
        string? OpenFile(string filter);
        List<string> OpenFiles(string filter);
        string? OpenFolder();
        void ShowDialog<TViewModel>(TViewModel modelInstance, Action<bool?, TViewModel>? action = null)
            where TViewModel : class;
    }

    public interface IDialogResult<T>
    {
        T Data { get; }
    }
}
