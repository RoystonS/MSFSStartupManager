using System;
using System.Windows.Input;

namespace MSFSStartupManager
{
    public abstract class WizardPageViewModelBase : ViewModelBase, IWizardPageViewModel
    {
        public abstract ICommand MoveNext { get; }
        public abstract string ViewName { get; }

        private bool canMovePrevious = true;

        public bool CanMovePrevious
        {
            get { return canMovePrevious; }
            set { SetProperty(ref canMovePrevious, value); }
        }

        public virtual void EnterPage()
        {
        }

        public virtual void ExitPage()
        {
        }
    }

    public class DisabledCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return false;
        }

        public void Execute(object parameter)
        {
        }
    }

    public class EnabledCommand : ICommand
    {
        private readonly ExecuteCommandDelegate execute;

        public EnabledCommand(ExecuteCommandDelegate execute)
        {
            this.execute = execute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            execute();
        }
    }

    public class WizardPageIntroViewModel : WizardPageViewModelBase
    {
        private readonly MainWindowViewModel mainViewModel;
        private readonly WizardPageInitialScanViewModel nextPageViewModel;

        public WizardPageIntroViewModel(MainWindowViewModel mainViewModel)
        {
            this.mainViewModel = mainViewModel;
            this.nextPageViewModel = new WizardPageInitialScanViewModel(mainViewModel);
        }

        public override string ViewName => "WizardPageIntro.xaml";

        private ICommand moveNext;

        public override ICommand MoveNext
        {
            get
            {
                if (moveNext == null)
                {
                    moveNext = new PropertyBasedCommand(this, nameof(CanMoveNext), DoMoveNext);
                }
                return moveNext;
            }
        }

        public bool CanMoveNext
        {
            get { return true; }
        }

        private void DoMoveNext()
        {
            mainViewModel.MoveForwardTo(nextPageViewModel);
        }
    }
}
