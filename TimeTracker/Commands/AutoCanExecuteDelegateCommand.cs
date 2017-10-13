using Prism.Commands;
using System;
using System.Windows.Input;

namespace TimeTracker.Commands
{
    public class AutoCanExecuteDelegateCommand : ICommand
    {
        public AutoCanExecuteDelegateCommand(Action executeMethod, Func<bool> canExecuteMethod) 
        {
            WrappedCommand = new DelegateCommand(executeMethod, canExecuteMethod);
        }

        public void Execute(object parameter)
        {
            WrappedCommand.Execute(parameter);
        }

        public bool CanExecute(object parameter)
        {
            return WrappedCommand.CanExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public ICommand WrappedCommand { get; private set; }
    }
}
