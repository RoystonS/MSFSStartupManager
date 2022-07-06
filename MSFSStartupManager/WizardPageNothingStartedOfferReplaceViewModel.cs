using System.Windows.Input;

namespace MSFSStartupManager
{
    public class WizardPageNothingStartedOfferReplaceViewModel : WizardPageViewModelBase
    {
        private readonly MainWindowViewModel mainViewModel;

        public WizardPageNothingStartedOfferReplaceViewModel(MainWindowViewModel mainViewModel)
        {
            this.mainViewModel = mainViewModel;
            mainViewModel.PropertyChanged += MainViewModel_PropertyChanged;
        }

        private void MainViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainWindowViewModel.TriedReplacingExeXml))
            {
                RaisePropertyChanged(nameof(YesTryReplacing));
                RaisePropertyChanged(nameof(NoDoNotReplace));
                RaisePropertyChanged(nameof(CanMoveNext));
            }
        }

        public bool YesTryReplacing
        {
            get { return mainViewModel.TriedReplacingExeXml.HasValue && mainViewModel.TriedReplacingExeXml.Value; }
            set
            {
                mainViewModel.TriedReplacingExeXml = value;
            }
        }

        public bool NoDoNotReplace
        {
            get { return mainViewModel.TriedReplacingExeXml.HasValue && !mainViewModel.TriedReplacingExeXml.Value; }
            set
            {
                mainViewModel.TriedReplacingExeXml = !value;
            }
        }

        public override ICommand MoveNext => new PropertyBasedCommand(this, nameof(CanMoveNext), DoMoveNext);

        public bool CanMoveNext
        {
            get { return mainViewModel.TriedReplacingExeXml.HasValue; }
        }

        private WizardPageNothingStartedReplaceViewModel nothingStartedReplaceViewModel;

        private void DoMoveNext()
        {
            if (YesTryReplacing)
            {
                if (nothingStartedReplaceViewModel == null)
                {
                    nothingStartedReplaceViewModel = new WizardPageNothingStartedReplaceViewModel(mainViewModel);
                }
                mainViewModel.MoveForwardTo(nothingStartedReplaceViewModel);
            }
            else
            {
                mainViewModel.MoveForwardTo(new WizardPageContactViewModel(mainViewModel));
            }
        }

        public override string ViewName => "WizardPageNothingStartedOfferReplace.xaml";
    }
}
