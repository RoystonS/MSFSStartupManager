using MSFSExeXml;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;

namespace MSFSStartupManager
{
    public class WizardPageNothingStartedReplaceViewModel : WizardPageViewModelBase
    {
        public override ICommand MoveNext => new PropertyBasedCommand(this, nameof(CanMoveNext), DoMoveNext);

        public override string ViewName => "WizardPageNothingStartedReplace.xaml";

        private readonly string backupExeXmlFilename = Path.Join(Path.GetDirectoryName(FlightSimulatorPaths.ExeXmlPath), "exe.msfs-startup-backup.xml");
        private readonly MainWindowViewModel mainViewModel;
        private bool firstEnter = true;

        public WizardPageNothingStartedReplaceViewModel(MainWindowViewModel mainViewModel)
        {
            this.mainViewModel = mainViewModel;
            mainViewModel.PropertyChanged += MainViewModel_PropertyChanged;
        }

        private void MainViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainWindowViewModel.ReplacementExeXmlWorked))
            {
                RaisePropertyChanged(nameof(DidStartUp));
                RaisePropertyChanged(nameof(DidNotStartUp));
                RaisePropertyChanged(nameof(CanMoveNext));
            }
        }

        public override void EnterPage()
        {
            base.EnterPage();

            var exeXmlPath = FlightSimulatorPaths.ExeXmlPath;

            var model = ExeXmlModel.CreateEmpty();
            var addon = model.AddAddon("Startup Test", Process.GetCurrentProcess().MainModule.FileName);
            addon.CommandLine = "/startuptest";

            if (File.Exists(exeXmlPath))
            {
                if (File.Exists(backupExeXmlFilename))
                {
                    File.Delete(backupExeXmlFilename);
                }
                File.Move(exeXmlPath, backupExeXmlFilename);
            }

            model.Save(exeXmlPath);

            if (firstEnter)
            {
                firstEnter = false;
                // Show backup file in explorer
                Process.Start("explorer.exe", "/select, \"" + backupExeXmlFilename + "\"");
            }
        }

        public override void ExitPage()
        {
            base.ExitPage();

            // Restore exe.xml to its original
            if (File.Exists(backupExeXmlFilename))
            {
                var exeXmlPath = FlightSimulatorPaths.ExeXmlPath;
                if (File.Exists(exeXmlPath))
                {
                    File.Delete(exeXmlPath);
                }
                File.Move(backupExeXmlFilename, exeXmlPath);
            }
        }

        public object CanMoveNext
        {
            get { return mainViewModel.ReplacementExeXmlWorked.HasValue; }
        }

        private void DoMoveNext()
        {
            mainViewModel.MoveForwardTo(new WizardPageContactViewModel(mainViewModel));
        }

        public bool DidStartUp
        {
            get
            {
                var didStartUp = mainViewModel.ReplacementExeXmlWorked;
                return didStartUp.HasValue && didStartUp.Value;
            }
            set
            {
                mainViewModel.ReplacementExeXmlWorked = value;
            }
        }

        public bool DidNotStartUp
        {
            get
            {
                var didStartUp = mainViewModel.ReplacementExeXmlWorked;
                return didStartUp.HasValue && !didStartUp.Value;
            }
            set
            {
                mainViewModel.ReplacementExeXmlWorked = !value;
            }
        }

        public static string StartupTestMessage
        {
            get { return "Congratulations, the addon startup test has worked."; }
        }
    }
}
