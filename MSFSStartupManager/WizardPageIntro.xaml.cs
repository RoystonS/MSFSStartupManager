﻿using System.Diagnostics;
using System.Windows.Controls;

namespace MSFSStartupManager
{
    /// <summary>
    /// Interaction logic for WizardPageStart.xaml
    /// </summary>
    public partial class WizardPageIntro : Page
    {
        public WizardPageIntro()
        {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            UrlOpener.OpenUrl(e.Uri);
            e.Handled = true;
        }
    }
}
