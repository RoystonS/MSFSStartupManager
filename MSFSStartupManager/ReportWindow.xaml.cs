using System.Windows;

namespace MSFSStartupManager
{
    /// <summary>
    /// Interaction logic for ReportWindow.xaml
    /// </summary>
    public partial class ReportWindow : Window
    {

        public ReportWindow()
        {
            InitializeComponent();

            ViewModel = new ReportWindowViewModel();
        }

        public ReportWindowViewModel ViewModel
        {
            get { return (ReportWindowViewModel)DataContext; }
            set { DataContext = value; }
        }
    }
}
