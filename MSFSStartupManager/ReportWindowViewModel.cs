using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Input;

namespace MSFSStartupManager
{
    public class ReportWindowViewModel : ViewModelBase
    {        
        public bool EnableControls
        {
            get { return !Submitting; }
        }

        private bool submitting = false;
        public bool Submitting
        {
            get { return submitting; }
            set
            {
                SetProperty(ref submitting, value);
                RaisePropertyChanged(nameof(EnableControls));
            }
        }

        private bool submitFailed = false;
        public bool SubmitFailed
        {
            get { return submitFailed; }
            set
            {
                SetProperty(ref submitFailed, value);
            }
        }

        private bool submittedSuccessfully = false;
        public bool SubmittedSuccessfully
        {
            get { return submittedSuccessfully; }
            set
            {
                SetProperty(ref submittedSuccessfully, value);
            }
        }

        public void SendReport()
        {
            Submitting = true;
        }


        private ICommand sendReportCommand = null;

        public ICommand SendReportCommand
        {
            get
            {
                if (sendReportCommand == null)
                {
                    
                }
                return sendReportCommand;
            }
            set
            {
                sendReportCommand = value;
            }
        }

        private class MoveToNext : ICommand
        {
            private readonly ReportWindowViewModel viewModel;
            public MoveToNext(ReportWindowViewModel viewModel)
            {
                this.viewModel = viewModel;
            }

            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                if (viewModel.Submitting)
                {
                    return false;
                }

                if (viewModel.SubmittedSuccessfully)
                {
                    return false;
                }


                return true;
            }

            public void Execute(object parameter)
            {
                Console.WriteLine("X {0}", Thread.CurrentThread.ManagedThreadId);

                viewModel.SubmitFailed = false;

                viewModel.Submitting = true;
                try
                {
                    // await CreateAndUploadPackage.Run(viewModel.OtherComments, viewModel.Email, viewModel.EmailAboutTechnical, viewModel.EmailSendUpdates);
                    viewModel.SubmittedSuccessfully = true;
                }
                catch (Exception)
                {
                    Console.WriteLine("E {0}", Thread.CurrentThread.ManagedThreadId);
                }
                finally
                {
                    viewModel.Submitting = false;
                }
            }
        }
    }
}
