using MSFSExeXml;
using MSFSStartupManager.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace MSFSStartupManager
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly Stack<IWizardPageViewModel> pageHistory = new();

        public MainWindowViewModel()
        {
            ExeXmlModel model = null;

            try
            {
                model = ExeXmlModel.Load(FlightSimulatorPaths.ExeXmlPath);
            }
            catch (Exception)
            {
                if (File.Exists(FlightSimulatorPaths.ExeXmlPath))
                {
                    var exeXmlContents = File.ReadAllText(FlightSimulatorPaths.ExeXmlPath);
                    var fixedXmlContents = ExeXmlFixer.TryFix(exeXmlContents);
                    if (fixedXmlContents == null)
                    {
                        // Can't be fixed.
                        ExeXmlIsCorruptAndNotFixable = true;
                    }
                    else
                    {
                        // Can be fixed.
                        ExeXmlIsCorruptButFixable = true;
                        model = ExeXmlModel.Load(new StringReader(fixedXmlContents));
                    }
                }
                else
                {
                    ExeXmlIsMissing = true;
                }
            }

            if (model != null)
            {
                AddonStartupStatusViewModels = (from addon in model.Addons select addon.Name).Select(n => new AddonStartupStatusReportViewModel(n)).ToArray();

                var enabledAddonNames = from addon in model.Addons
                                        where !addon.Disabled
                                        select addon.Name;

                if (!enabledAddonNames.Any())
                {
                    // Exe.xml is ok, but there are no addons configured. So user won't know whether
                    // it's working or not.
                    HasNoEnabledAddons = true;
                }
                EnabledAddonsNames = String.Join(", ", enabledAddonNames);
            }
            else
            {
                AddonStartupStatusViewModels = new AddonStartupStatusReportViewModel[] { };
                HasNoEnabledAddons = true;
                EnabledAddonsNames = "";
            }

            CurrentPageViewModel = new WizardPageIntroViewModel(this);
        }

        private ICommand movePrevious;

        public ICommand MovePrevious
        {
            get
            {
                if (movePrevious == null)
                {
                    movePrevious = new PropertyBasedCommand(this, nameof(CanMovePrevious), DoMovePrevious);
                }
                return movePrevious;
            }
        }

        public bool CanMovePrevious
        {
            get
            {
                if (pageHistory.Count == 0)
                {
                    return false;
                }

                if (CurrentPageViewModel == null)
                {
                    return false;
                }

                return CurrentPageViewModel.CanMovePrevious;
            }
        }

        private void DoMovePrevious()
        {
            var page = pageHistory.Pop();
            CurrentPageViewModel = page;
        }

        public void MoveForwardTo(IWizardPageViewModel pageViewModel)
        {
            if (CurrentPageViewModel != null)
            {
                pageHistory.Push(CurrentPageViewModel);
            }
            CurrentPageViewModel = pageViewModel;
        }

        private IWizardPageViewModel currentPageViewModel;
        public IWizardPageViewModel CurrentPageViewModel
        {
            get { return currentPageViewModel; }
            set
            {
                if (currentPageViewModel == value)
                {
                    return;
                }

                if (currentPageViewModel != null)
                {
                    currentPageViewModel.PropertyChanged -= CurrentPageViewModel_PropertyChanged;
                    currentPageViewModel.ExitPage();
                }
                if (value != null)
                {
                    value.PropertyChanged += CurrentPageViewModel_PropertyChanged;
                    value.EnterPage();
                }

                SetProperty(ref currentPageViewModel, value);
                RaisePropertyChanged(nameof(CanMovePrevious));
            }
        }

        private void CurrentPageViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IWizardPageViewModel.CanMovePrevious))
            {
                RaisePropertyChanged(nameof(CanMovePrevious));
            }
        }

        public bool HasNoEnabledAddons
        {
            get; private set;
        }

        public bool ExeXmlIsMissing
        {
            get; private set;
        }

        public bool ExeXmlIsCorruptButFixable
        {
            get; private set;
        }

        public bool ExeXmlIsCorruptAndNotFixable
        {
            get; private set;
        }

        public string EnabledAddonsNames
        {
            get; private set;
        }

        public AddonStartupStatusReportViewModel[] AddonStartupStatusViewModels
        {
            get;
            private set;
        }

        public bool WeWantUserToSendUsAReport
        {
            get { return ExeXmlIsCorruptButFixable || !HasNoEnabledAddons; }
        }

        private string email = "";

        public string Email
        {
            get { return email; }
            set { SetProperty(ref email, value); }
        }

        private bool emailAboutTechnical = false;
        public bool EmailAboutTechnical
        {
            get { return emailAboutTechnical; }
            set { SetProperty(ref emailAboutTechnical, value); }
        }

        private bool emailSendUpdates = false;
        public bool EmailSendUpdates
        {
            get { return emailSendUpdates; }
            set { SetProperty(ref emailSendUpdates, value); }
        }

        private string otherComments = "";
        public string OtherComments
        {
            get { return otherComments; }
            set { SetProperty(ref otherComments, value); }
        }

        private bool? triedReplacingExeXml;
        public bool? TriedReplacingExeXml
        {
            get { return triedReplacingExeXml; }
            set { SetProperty(ref triedReplacingExeXml, value); }
        }

        private bool? replacementExeXmlWorked;
        public bool? ReplacementExeXmlWorked
        {
            get { return replacementExeXmlWorked; }
            set { SetProperty(ref replacementExeXmlWorked, value); }
        }

        public void Shutdown()
        {
            CurrentPageViewModel = null;
        }
    }
}
