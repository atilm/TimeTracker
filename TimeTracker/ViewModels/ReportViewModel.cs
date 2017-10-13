﻿using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using TimeTracker.DbMappings;

namespace TimeTracker.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ReportViewModel : BindableBase
    {
        [ImportingConstructor]
        public ReportViewModel(IProjectDataRepository repository)
        {
            this.repository = repository;
            ProjectList = new ObservableCollection<ReportItem>();
            SelectedDate = DateTime.Now;
            UpdateProjectList();

            repository.RaiseRecordsChangedEvent += OnRepositoryChanged;
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(SelectedDate))
                UpdateProjectList();
        }

        public DateTime SelectedDate
        {
            get { return selectedDate; }
            set { SetProperty(ref selectedDate, value); }
        }

        public string OvertimeString
        {
            get
            {
                var appData = repository.GetAppData();
                return $"{appData.OvertimeHours} h";
            }
        }

        public ObservableCollection<ReportItem> ProjectList
        {
            get { return projectList; }
            set { projectList = value; }
        }

        private void OnRepositoryChanged(object sender, EventArgs e)
        {
            UpdateProjectList();
        }

        private void UpdateProjectList()
        {
            var records = repository.GetRecords(SelectedDate);

            ProjectList.Clear();

            foreach (var record in records)
            {
                var project = record.Task.Project;

                var reportItem = ProjectList
                    .FirstOrDefault(item => item.BelongsToProject(record.Task.Project));

                if (reportItem == null)
                {
                    reportItem = new ReportItem();
                    reportItem.Project = project;
                    ProjectList.Add(reportItem);
                }

                reportItem.Duration += record.Duration;
            }
        }

        private IProjectDataRepository repository;
        private DateTime selectedDate;
        private ObservableCollection<ReportItem> projectList;
    }
}
