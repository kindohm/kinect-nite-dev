using System;
using System.Windows.Input;

namespace CursorClicking2
{
    public class AwesomeCommand : ICommand
    {
        Action action;

        public AwesomeCommand(Action action)
        {
            this.action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            action.Invoke();        
        }
    }
}
