using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSFSStartupManager.UI
{
    public class AddonStartupStatusReportViewModel : ViewModelBase
    {
        public AddonStartupStatusReportViewModel(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }

        private string comments;
        public string Comments
        {
            get { return comments; }
            set { SetProperty(ref comments, value); }
        }

        private bool? started;
        public bool? Started
        {
            get { return started; }
            set
            {
                SetProperty(ref started, value);
                RaisePropertyChanged(nameof(ConfirmedStarted));
                RaisePropertyChanged(nameof(ConfirmedNotStarted));
            }
        }

        public bool ConfirmedStarted
        {
            get { return started.HasValue && started.Value; }
            set
            {
                Started = value;
            }
        }
        public bool ConfirmedNotStarted
        {
            get { return started.HasValue && !started.Value; }
            set
            {
                Started = !value;
            }
        }
    }
}
