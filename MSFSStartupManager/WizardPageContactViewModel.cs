using System.Windows.Input;

namespace MSFSStartupManager
{
    public class WizardPageContactViewModel : WizardPageViewModelBase
    {
        private readonly MainWindowViewModel mainViewModel;

        public WizardPageContactViewModel(MainWindowViewModel mainViewModel)
        {
            this.mainViewModel = mainViewModel;
            mainViewModel.PropertyChanged += MainViewModel_PropertyChanged;
        }

        public override string ViewName => "WizardPageContact.xaml";

        private void MainViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(MainWindowViewModel.Email):
                    RaisePropertyChanged(nameof(Email));
                    RaisePropertyChanged(nameof(HasProvidedEmail));
                    RaisePropertyChanged(nameof(CanMoveNext));
                    break;
                case nameof(MainWindowViewModel.EmailAboutTechnical):
                    RaisePropertyChanged(nameof(EmailAboutTechnical));
                    RaisePropertyChanged(nameof(CanMoveNext));
                    break;
                case nameof(MainWindowViewModel.EmailSendUpdates):
                    RaisePropertyChanged(nameof(EmailSendUpdates));
                    RaisePropertyChanged(nameof(CanMoveNext));
                    break;
                case nameof(MainWindowViewModel.OtherComments):
                    RaisePropertyChanged(nameof(OtherComments));
                    break;
            }

            if (e.PropertyName != nameof(MainWindowViewModel.CanMovePrevious))
            {
                CanMovePrevious = CanMoveNext;
            }
        }

        public string Email
        {
            get { return mainViewModel.Email; }
            set
            {
                mainViewModel.Email = value;
            }
        }

        public bool HasProvidedEmail
        {
            get { return !string.IsNullOrEmpty(Email.Trim()); }
        }

        public bool EmailAboutTechnical
        {
            get { return mainViewModel.EmailAboutTechnical; }
            set { mainViewModel.EmailAboutTechnical = value; }
        }

        public bool EmailSendUpdates
        {
            get { return mainViewModel.EmailSendUpdates; }
            set { mainViewModel.EmailSendUpdates = value; }
        }

        public string OtherComments
        {
            get { return mainViewModel.OtherComments; }
            set { mainViewModel.OtherComments = value; }
        }

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
            get
            {
                if (HasProvidedEmail && (!EmailAboutTechnical && !EmailSendUpdates))
                {
                    // They've provided an email address but not given us permission to do anything with it.
                    return false;
                }

                return true;
            }
        }

        private void DoMoveNext()
        {
            mainViewModel.MoveForwardTo(new WizardPageSummaryAndSendViewModel(mainViewModel));
        }
    }
}
