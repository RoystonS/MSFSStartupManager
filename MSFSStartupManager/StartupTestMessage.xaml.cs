using System.Windows;

namespace MSFSStartupManager
{
    /// <summary>
    /// Interaction logic for StartupTestMessage.xaml
    /// </summary>
    public partial class StartupTestMessage : Window
    {
        public StartupTestMessage()
        {
            InitializeComponent();

            this.Message.Text = WizardPageNothingStartedReplaceViewModel.StartupTestMessage;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
