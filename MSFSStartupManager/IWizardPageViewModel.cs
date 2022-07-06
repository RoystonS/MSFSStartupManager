using System.ComponentModel;
using System.Windows.Input;

namespace MSFSStartupManager
{
    public interface IWizardPageViewModel : INotifyPropertyChanged
    {
        string ViewName { get; }
        bool CanMovePrevious { get; }
        ICommand MoveNext { get; }

        void EnterPage();
        void ExitPage();
    }
}
