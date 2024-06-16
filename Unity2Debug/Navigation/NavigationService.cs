using System.Windows.Controls;

namespace Unity2Debug.Navigation
{
    internal class NavigationService
    {
        private readonly Frame _mainFrame;

        public static NavigationService? Instance { get; private set; }

        public NavigationService(Frame mainFrame)
        {
            _mainFrame = mainFrame;
            Instance = this;
        }

        public void NavigateTo(Page page)
        {
            _mainFrame.Navigate(page);
        }

        public void Back()
        {
            _mainFrame.GoBack();
        }
    }
}
