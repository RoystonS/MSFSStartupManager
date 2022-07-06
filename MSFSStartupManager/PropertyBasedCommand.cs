using System;
using System.ComponentModel;
using System.Windows.Input;

namespace MSFSStartupManager
{
    public delegate void ExecuteCommandDelegate();

    public class PropertyBasedCommand : ICommand
    {
        private readonly INotifyPropertyChanged obj;
        private readonly string enabledPropertyName;
        private readonly ExecuteCommandDelegate execute;

        public PropertyBasedCommand(INotifyPropertyChanged obj, string enabledPropertyName, ExecuteCommandDelegate execute)
        {
            this.obj = obj;
            this.enabledPropertyName = enabledPropertyName;
            this.execute = execute;
        }

        private EventHandler listeners;

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (listeners == null)
                {
                    obj.PropertyChanged += Obj_PropertyChanged;
                }
                listeners = (EventHandler)Delegate.Combine(listeners, value);
            }
            remove
            {
                listeners = (EventHandler)Delegate.Remove(listeners, value);
                if (listeners == null)
                {
                    obj.PropertyChanged -= Obj_PropertyChanged;
                }
            }
        }

        private void Obj_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == enabledPropertyName)
            {
                listeners?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool CanExecute(object parameter)
        {
            return (bool)obj.GetType().GetProperty(enabledPropertyName).GetValue(obj);
        }

        public void Execute(object parameter)
        {
            execute();
        }
    }
}
