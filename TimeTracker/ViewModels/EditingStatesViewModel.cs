using Prism.Commands;
using Prism.Mvvm;
using System.Windows.Input;
using TimeTracker.Commands;
using System;

namespace TimeTracker.ViewModels
{
    public abstract class EditingStatesViewModel : BindableBase
    {
        public enum StatusEnum
        {
            Idle,
            Adding,
            Editing
        }

        public EditingStatesViewModel()
        {
            AcceptEditingCommand = new AutoCanExecuteDelegateCommand(OnAcceptEditing, CanAcceptEditing);
            CancelEditingCommand = new DelegateCommand(OnCancelEditing);
            EditCommand = new AutoCanExecuteDelegateCommand(OnEdit, CanEdit);
            AddCommand = new AutoCanExecuteDelegateCommand(OnAdd, CanAdd);
            DeleteCommand = new AutoCanExecuteDelegateCommand(OnDelete, CanDelete);
        }

        public ICommand AcceptEditingCommand { get; set; }
        public ICommand CancelEditingCommand { get; set; }
        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public StatusEnum Status
        {
            get { return status; }
            set
            {
                SetProperty(ref status, value);
                RaisePropertyChanged(nameof(IsEditing));
                RaisePropertyChanged(nameof(IsNotEditing));
            }
        }

        public bool IsEditing
        {
            get { return Status != StatusEnum.Idle; }
        }

        public bool IsNotEditing
        {
            get { return !IsEditing; }
        }

        protected virtual bool CanAcceptEditing()
        {
            return true;
        }

        protected virtual bool CanDelete()
        {
            return true;
        }

        protected virtual bool CanEdit()
        {
            return true;
        }

        protected virtual bool CanAdd()
        {
            return true;
        }

        protected abstract void Delete();

        protected abstract void Add();

        protected abstract void Edit();

        protected abstract void AcceptEditing();

        protected abstract void CancelEditing();

        private void OnCancelEditing()
        {
            CancelEditing();
            Status = StatusEnum.Idle;
        }

        private void OnAcceptEditing()
        {
            AcceptEditing();
            Status = StatusEnum.Idle;
        }

        private void OnDelete()
        {
            Delete();
        }

        private void OnAdd()
        {
            Status = StatusEnum.Adding;
            Add();
        }

        private void OnEdit()
        {
            Status = StatusEnum.Editing;
            Edit();
        }

        private StatusEnum status;
    }
}
