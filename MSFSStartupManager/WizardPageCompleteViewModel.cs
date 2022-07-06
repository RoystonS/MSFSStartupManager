using System.Windows.Input;

namespace MSFSStartupManager
{
    public class WizardPageCompleteViewModel : WizardPageViewModelBase
    {
        private readonly MainWindowViewModel mainViewModel;

        public WizardPageCompleteViewModel(MainWindowViewModel mainViewModel)
        {
            this.mainViewModel = mainViewModel;

            // If we're at this page, we've done the submission.
            this.CanMovePrevious = false;
        }

        public override ICommand MoveNext => new DisabledCommand();

        public override string ViewName => "WizardPageComplete.xaml";
    }
}