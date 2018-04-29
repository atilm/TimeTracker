using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using TimeTracker.DbMappings;
using TimeTracker.DomainWrappers.ObjectWrappers;

namespace TimeTracker.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ProjectsViewModel : EditingStatesViewModel
    {
        [ImportingConstructor]
        public ProjectsViewModel(IProjectDataRepository repository)
        {
            Repository = repository;
            Repository.RaiseProjectsChangedEvent += OnProjectsChanged;
            Repository.RaiseTasksChangedEvent += OnTasksChanged;

            Projects = new ObservableCollection<ProjectVM>();
            EditingProject = new ProjectVM();
            LoadProjects();
        }

        private void OnProjectsChanged(object sender, EventArgs e)
        {
            UpdateProjectList();
        }

        private void OnTasksChanged(object sender, EventArgs e)
        {
            UpdateProjectList();
        }

        private void UpdateProjectList()
        {
            ClearProjectList();
            LoadProjects();
        }

        private void ClearProjectList()
        {
            foreach(var project in Projects)
                project.PropertyChanged -= Project_PropertyChanged;

            Projects.Clear();
        }

        private void LoadProjects()
        {
            var projects = Repository.GetProjects();

            foreach (var project in projects)
                AddProject(project);
        }

        private void AddProject(ProjectVM project)
        {
            Projects.Add(project);
            project.PropertyChanged += Project_PropertyChanged;
        }

        private void Project_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(ProjectVM.IsActive))
            {
                var project = sender as ProjectVM;
                Repository.SaveOrUpdate(project);
            }
        }

        public ObservableCollection<ProjectVM> Projects { get; protected set; }

        public ProjectVM EditingProject
        {
            get { return editingProject; }
            set { SetProperty(ref editingProject, value); }
        }

        public ProjectVM CurrentProject
        {
            get { return currentProject; }
            set { SetProperty(ref currentProject, value); }
        }

        protected override void Edit()
        {
            EditingProject.Name = CurrentProject.Name;
            EditingProject.ProjectNumber = CurrentProject.ProjectNumber;
        }

        protected override bool CanDelete()
        {
            return CurrentProject != null &&
                   CurrentProject.Tasks.Count == 0;
        }

        protected override bool CanEdit()
        {
            return CurrentProject != null;
        }

        protected override void AcceptEditing()
        {
            if (Status == StatusEnum.Adding)
                CreateNewProject();

            CopyEditContentsToCurrentProject();
            Repository.SaveOrUpdate(CurrentProject);
            ClearEditingProject();
        }

        private void CreateNewProject()
        {
            CurrentProject = new ProjectVM();
            AddProject(CurrentProject);
        }

        private void CopyEditContentsToCurrentProject()
        {
            CurrentProject.Name = EditingProject.Name;
            CurrentProject.ProjectNumber = EditingProject.ProjectNumber;
        }

        protected override void CancelEditing()
        {
            ClearEditingProject();
        }

        private void ClearEditingProject()
        {
            EditingProject.Name = "";
            EditingProject.ProjectNumber = "";
        }

        protected override void Add()
        {
            EditingProject = new ProjectVM();
        }

        protected override void Delete()
        {
            CurrentProject.PropertyChanged -= Project_PropertyChanged;
            Repository.Delete(CurrentProject);
            Projects.Remove(CurrentProject);
        }

        private ProjectVM editingProject;
        private ProjectVM currentProject;
        private IProjectDataRepository Repository;
    }
}
