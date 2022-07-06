using System;
using System.Windows;

namespace MSFSStartupManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ViewModel = new MainWindowViewModel();
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            UpdateCurrentPage();
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainWindowViewModel.CurrentPageViewModel))
            {
                UpdateCurrentPage();
            }
        }

        private void UpdateCurrentPage()
        {
            var viewModel = ViewModel.CurrentPageViewModel;
            if (viewModel != null)
            {
                NavigationFrame.Navigate(new Uri(viewModel.ViewName, UriKind.Relative));
            }
        }

        public MainWindowViewModel ViewModel
        {
            get { return (MainWindowViewModel)DataContext; }
            set { DataContext = value; }
        }

        private void NavigationFrame_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            var content = NavigationFrame.Content as FrameworkElement;
            if (content == null)
            {
                return;
            }
            content.DataContext = ViewModel.CurrentPageViewModel;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ViewModel != null)
            {
                ViewModel.Shutdown();
            }
        }
    }
}
