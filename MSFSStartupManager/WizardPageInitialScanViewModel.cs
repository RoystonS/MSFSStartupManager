using MSFSStartupManager.UI;
using System.Linq;
using System.Windows.Input;

namespace MSFSStartupManager
{
    public class WizardPageInitialScanViewModel : WizardPageViewModelBase
    {
        private readonly MainWindowViewModel mainViewModel;

        public WizardPageInitialScanViewModel(MainWindowViewModel mainViewModel)
        {
            this.mainViewModel = mainViewModel;
            mainViewModel.PropertyChanged += MainViewModel_PropertyChanged;

            foreach (var addonViewModel in mainViewModel.AddonStartupStatusViewModels)
            {
                addonViewModel.PropertyChanged += AddonViewModel_PropertyChanged;
            }
        }

        public AddonStartupStatusReportViewModel[] AddonStartupStatusViewModels
        {
            get { return mainViewModel.AddonStartupStatusViewModels; }
        }

        private void MainViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
        }

        private void AddonViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(CanMoveToNext));
        }

        public override string ViewName => "WizardPageInitialScan.xaml";


        private ICommand moveNextCommand;
        public override ICommand MoveNext
        {
            get
            {
                if (moveNextCommand == null)
                {
                    moveNextCommand = new PropertyBasedCommand(this, nameof(CanMoveToNext), DoMoveToNext);
                }
                return moveNextCommand;
            }
        }

        public bool CanMoveToNext
        {
            get { return AddonStartupStatusViewModels.All(vm => vm.Started.HasValue); }
        }

        private WizardPageNothingStartedOfferReplaceViewModel offerReplaceViewModel;

        private void DoMoveToNext()
        {
            if (AddonStartupStatusViewModels.All(vm => vm.Started == false))
            {
                // NOTHING started up, so there's some fundamental problem. Offer to replace their exe.xml completely
                if (offerReplaceViewModel == null)
                {
                    offerReplaceViewModel = new WizardPageNothingStartedOfferReplaceViewModel(mainViewModel);
                }
                mainViewModel.MoveForwardTo(offerReplaceViewModel);
            }
            else
            {
                mainViewModel.MoveForwardTo(new WizardPageContactViewModel(mainViewModel));
            }
        }

        public string EnabledAddonsNames
        {
            get { return mainViewModel.EnabledAddonsNames; }
        }

        public bool HasNoEnabledAddons
        {
            get { return mainViewModel.HasNoEnabledAddons; }
        }

        public bool ExeXmlIsCorruptAndNotFixable
        {
            get { return mainViewModel.ExeXmlIsCorruptAndNotFixable; }
        }
    }
}
