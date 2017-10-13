using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using TimeTracker.DbMappings;
using TimeTracker.DomainWrappers.ObjectWrappers;

namespace TimeTracker.ViewModels
{
    public abstract class TasksViewModel : EditingStatesViewModel
    {
        public TasksViewModel(IProjectDataRepository repository)
        {
            this.repository = repository;

            EditingTask = new TaskVM();
            Tasks = new ObservableCollection<TaskVM>();

            UpdateProjectList();
            LoadTasks();
            AttachEvents();
        }

        private void AttachEvents()
        {
            repository.RaiseTasksChangedEvent += OnTasksChanged;
            repository.RaiseProjectsChangedEvent += OnProjectsChanged;
        }

        private void OnProjectsChanged(object sender, EventArgs e)
        {
            UpdateProjectList();
        }

        private void OnTasksChanged(object sender, EventArgs e)
        {
            LoadTasks();
        }

        protected virtual void LoadTasks()
        {
            ClearTaskList();

            var tasks = GetTasksFromRepository();

            foreach(var task in tasks)
                AddTask(task);
        }

        protected abstract ObservableCollection<TaskVM> GetTasksFromRepository();

        private void ClearTaskList()
        {
            foreach (var task in Tasks)
                task.PropertyChanged -= Task_PropertyChanged;

            Tasks.Clear();
        }

        private void AddTask(TaskVM task)
        {
            Tasks.Add(task);
            task.PropertyChanged += Task_PropertyChanged;
        }

        private void Task_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(TaskVM.IsDone))
            {
                var task = sender as TaskVM;
                repository.SaveOrUpdate(task);
            }
        }

        public ObservableCollection<ProjectVM> Projects
        {
            get { return projects; }
            protected set { SetProperty(ref projects, value); }
        }

        public ObservableCollection<TaskVM> Tasks
        {
            get { return tasks; }
            protected set { SetProperty(ref tasks, value); }
        }


        public TaskVM EditingTask
        {
            get { return editingTask; }
            set { SetProperty(ref editingTask, value); }
        }


        public TaskVM CurrentTask
        {
            get { return currentTask; }
            set { SetProperty(ref currentTask, value); }
        }

        protected override void Add()
        {
            UpdateProjectList();
        }

        protected override bool CanEdit()
        {
            return CurrentTask != null;
        }

        protected override void Edit()
        {
            UpdateProjectList();
            EditingTask.Project = Projects.First(p => p.Id == CurrentTask.Project.Id);
            EditingTask.Name = CurrentTask.Name;
        }

        protected override bool CanAdd()
        {
            return Projects.Count != 0;
        }

        protected override bool CanDelete()
        {
            return CurrentTask != null;
        }

        protected override void Delete()
        {
            RemoveTask(CurrentTask);
            CurrentTask = null;
        }

        private void RemoveTask(TaskVM currentTask)
        {
            CurrentTask.PropertyChanged -= Task_PropertyChanged;
            repository.Delete(CurrentTask);
            Tasks.Remove(CurrentTask);
        }

        protected override void AcceptEditing()
        {
            if (Status == StatusEnum.Adding)
                CreateNewTask();

            CopyEditContentsToCurrentTask();
            repository.SaveOrUpdate(CurrentTask);
            ClearEditingTask();
        }

        private void ClearEditingTask()
        {
            EditingTask.Project = null;
            EditingTask.Name = "";
        }

        private void CopyEditContentsToCurrentTask()
        {
            CurrentTask.Project = EditingTask.Project;
            CurrentTask.Name = EditingTask.Name;
        }

        private void CreateNewTask()
        {
            CurrentTask = new TaskVM
            {
                IsDone = false
            };
            AddTask(CurrentTask);
        }

        protected override void CancelEditing()
        {
            
        }

        private void UpdateProjectList()
        {
            Projects = repository.GetActiveProjects();
        }

        protected IProjectDataRepository repository;
        protected TaskVM editingTask;
        protected TaskVM currentTask;
        protected ObservableCollection<TaskVM> tasks;
        protected ObservableCollection<ProjectVM> projects;
    }
}
