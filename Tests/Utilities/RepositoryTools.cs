using System;
using System.Linq;
using TimeTracker.DbMappings;
using TimeTracker.DomainWrappers.ObjectWrappers;

namespace Tests.Utilities
{
    public class RepositoryTools
    {
        public RepositoryTools InRepository(IProjectDataRepository repository)
        {
            this.repository = repository;
            return this;
        }

        public RepositoryTools InProject(string name)
        {
            currentProject = GetOrCreateProject(name);

            return this;
        }

        public RepositoryTools InProject(string name, string number, bool isActive)
        {
            InProject(name).SetProjectInfo(number, isActive);

            return this;
        }

        public RepositoryTools SetProjectInfo(string number, bool isActive)
        {
            currentProject.ProjectNumber = number;
            currentProject.IsActive = isActive;
            repository.SaveOrUpdate(currentProject);
            return this;
        }

        public RepositoryTools AddTask(string taskName)
        {
            return InTask(taskName);
        }

        public RepositoryTools InTask(string taskName)
        {
            currentTask = GetOrCreateTask(taskName);
            return this;
        }

        public RepositoryTools InTask(string name, DateTime doneDate)
        {
            InTask(name)
                .SetTaskDone(doneDate);
            return this;
        }

        public RepositoryTools SetTaskDone(DateTime doneDate)
        {
            currentTask.DoneDate = doneDate;
            currentTask.IsDone = true;
            repository.SaveOrUpdate(currentTask);
            return this;
        }

        public RepositoryTools AddRecord(DateTime start, DateTime stop)
        {
            currentTask.AddRecord(new RecordVM
            {
                Start = start,
                Stop = stop
            });

            repository.SaveOrUpdate(currentTask);
            return this;
        }

        private TaskVM GetOrCreateTask(string taskName)
        {
            var task = currentProject.Tasks.FirstOrDefault(t => t.Name == taskName);

            if (task == null)
            {
                task = BuildTask(currentProject, taskName, null);
                repository.SaveOrUpdate(task);
            }

            return task;
        }

        private ProjectVM GetOrCreateProject(string projectName)
        {
            var projects = repository.GetProjects();
            var project = projects.FirstOrDefault(p => p.Name == projectName);

            if (project == null)
            {
                project = BuildProject(projectName, "", true);
                repository.SaveOrUpdate(project);
            }

            return project;
        }

        private static ProjectVM BuildProject(string name, string number, bool isActive)
        {
            return new ProjectVM
            {
                Name = name,
                ProjectNumber = number,
                IsActive = isActive
            };
        }

        private static TaskVM BuildTask(ProjectVM project, string name, DateTime? doneDate)
        {
            return new TaskVM
            {
                Project = project,
                Name = name,
                DoneDate = doneDate ?? new DateTime(),
                IsDone = doneDate != null
            };
        }

        private IProjectDataRepository repository;
        private ProjectVM currentProject;
        private TaskVM currentTask;
    }
}
