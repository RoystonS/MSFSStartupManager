using System;
using System.Reflection;
using System.Windows;

namespace MSFSStartupManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (e.Args.Length > 0 && e.Args[0] == "/startuptest")
            {
                new StartupTestMessage().ShowDialog();
                return;
            }
            CheckSupportedThenStart();
        }

        private async void CheckSupportedThenStart()
        {
            // Is this program still supported?
            try
            {
                var status = await CreateAndUploadPackage.GetStatus();
                ProcessStatusNonDispatcherThread(status);
            }
            catch (NoLongerSupportedException)
            {
                ReportStatusNotFetchableNonDispatcherThread(true);
            }
            catch (Exception)
            {
                ReportStatusNotFetchableNonDispatcherThread(false);
            }
        }

        private void ReportStatusNotFetchableNonDispatcherThread(bool noLongerSupported)
        {
            Dispatcher.Invoke(() =>
            {
                var message = noLongerSupported ? "This application is no longer supported by the online services it used." : "Could not communicate with online services. Please check you are connected to the internet";
                var caption = noLongerSupported ? "No longer supported" : "Communications failure";        
                MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            });
        }

        private void ProcessStatusNonDispatcherThread(StatusResponse status)
        {
            Dispatcher.Invoke(() =>
            {
                if (!status.supported)
                {
                    MessageBox.Show("Please check for a newer version of this program. This version is no longer supported by the online services it used.", "No longer supported", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var version = Assembly.GetExecutingAssembly().GetName().Version;
                var currentProgramVersion = $"{version.Major}.{version.Minor}.{version.Build}";
                if (currentProgramVersion != status.requiredVersion)
                {
                    MessageBox.Show($"Please upgrade to version {status.requiredVersion} of this program. You currently have {currentProgramVersion}.", "Upgrade needed", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var window = new MainWindow();
                window.ShowDialog();
            });
        }
    }
}
