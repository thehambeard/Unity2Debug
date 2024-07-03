using System.Windows.Controls;
using Unity2Debug.Pages.ViewModel;

namespace Unity2Debug.Navigation
{
    internal class NavigationService
    {
        private readonly Frame _mainFrame;
        private readonly Stack<Page> _pageStack;
        public static NavigationService? Instance { get; private set; }

        public NavigationService(Frame mainFrame)
        {
            _mainFrame = mainFrame;
            _pageStack = new();
            Instance = this;
        }

        public void NavigateTo(Page page)
        {
            _pageStack.Push(page);
            _mainFrame.Navigate(page);
        }

        public void Back()
        {
            if (_pageStack.Count > 1)
            {
                _pageStack.Pop();

                var previousPage = _pageStack.Peek();

                if (previousPage != null && previousPage.DataContext is ViewModelBase vmBase)
                    vmBase.ValidateProfile();

                _mainFrame.Navigate(previousPage);
            }
        }
    }
}
