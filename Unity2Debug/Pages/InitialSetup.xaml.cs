﻿using System.Windows.Controls;
using Unity2Debug.Logging;
using Unity2Debug.Pages.ViewModel;

namespace Unity2Debug.Pages
{
    /// <summary>
    /// Interaction logic for InitialSetup.xaml
    /// </summary>
    public partial class InitialSetup : Page
    {
        private readonly TextBoxLogger _logger;
        private readonly InitialSetupVM _vm;

        public InitialSetup()
        {
            InitializeComponent();

            _logger = new TextBoxLogger(TextBoxLogger);
            _vm = new InitialSetupVM(_logger, new DialogService.DialogService());
            DataContext = _vm;
        }
    }
}
