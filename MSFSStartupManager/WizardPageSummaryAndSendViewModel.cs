using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MSFSStartupManager
{
    public class WizardPageSummaryAndSendViewModel : WizardPageViewModelBase
    {
        private readonly MainWindowViewModel mainViewModel;

        public WizardPageSummaryAndSendViewModel(MainWindowViewModel mainViewModel)
        {
            this.mainViewModel = mainViewModel;
        }

        public override ICommand MoveNext => new PropertyBasedCommand(this, nameof(CanMoveNext), DoMoveNext);

        public override string ViewName => "WizardPageSummaryAndSend.xaml";

        public string ReportText
        {
            get
            {
                return GenerateReport.BuildReport(mainViewModel);
            }
        }

        private bool canMoveNext = true;
        public bool CanMoveNext
        {
            get { return canMoveNext; }
            set { SetProperty(ref canMoveNext, value); }
        }

        private string errorMessage;
        public string ErrorMessage
        {
            get { return errorMessage; }
            set { SetProperty(ref errorMessage, value); }
        }

        private void DoMoveNext()
        {
            _ = SendAndMove();
        }

        private bool submitting;
        public bool IsSubmitting
        {
            get { return submitting; }
            private set
            {
                SetProperty(ref submitting, value);
            }
        }

        private async Task SendAndMove()
        {
            try
            {
                CanMoveNext = false;
                ErrorMessage = "";
                IsSubmitting = true;
                await CreateAndUploadPackage.Run(mainViewModel);
                mainViewModel.MoveForwardTo(new WizardPageCompleteViewModel(this.mainViewModel));
            }
            catch (Exception ex)
            {
                ErrorMessage = "Could not send report. Are you connected to the Internet? " + ex.Message;
            }
            finally
            {
                CanMoveNext = true;
                IsSubmitting = false;
            }
        }
    }
}
