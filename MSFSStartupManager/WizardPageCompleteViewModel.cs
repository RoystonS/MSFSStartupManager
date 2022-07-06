using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MSFSStartupManager
{
    public class WizardPageCompleteViewModel : WizardPageViewModelBase
    {
        private readonly MainWindowViewModel mainViewModel;

        public WizardPageCompleteViewModel(MainWindowViewModel mainViewModel)
        {
            this.mainViewModel = mainViewModel;
        }

        public override ICommand MoveNext => new DisabledCommand();

        public override string ViewName => "WizardPageComplete.xaml";
    }
}