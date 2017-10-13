using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TimeTracker.DbMappings;
using System.Linq;
using System;
using TimeTracker.DomainWrappers.ObjectWrappers;

namespace Tests
{
    [TestClass]
    public class DbTest
    {
        [TestInitialize]
        public void Init()
        {
            repository = new ProjectDataRepository(true);

            records.Clear();
            tasks.Clear();

            for (int i = 0; i < 4; i++)
            {
                var record = new RecordVM();
                records.Add(record);
            }

            for (int i = 0; i < 2; i++)
            {
                var task = new TaskVM
                {
                    Name = $"Task {i}"
                };
                tasks.Add(task);
            }
                

            records.First().Start = start;
            records.First().Stop = stop;

            tasks[0].AddRecord(records[0]);
            tasks[1].AddRecord(records[1]);
            tasks[1].AddRecord(records[2]);
            tasks[1].AddRecord(records[3]);

            projects.Add(new ProjectVM());
            projects[0].AddTask(tasks[0]);
            projects[0].AddTask(tasks[1]);
        }

        [TestCleanup]
        public void CleanUp()
        {
            repository.CloseSession();
        }

        [TestMethod]
        public void AddProjectTest()
        {
            projects[0].Name = "TestProject";
            projects[0].ProjectNumber = "OR029384";

            repository.SaveOrUpdate(projects[0]);

            var fromDb = repository.GetProjectByNumber(projects[0].ProjectNumber);

            Assert.AreEqual(projects[0].Id, fromDb.Id);
            Assert.AreEqual(projects[0].ProjectNumber, fromDb.ProjectNumber);
            Assert.AreEqual(projects[0].Name, fromDb.Name);
            Assert.AreEqual(2, fromDb.Tasks.Count);
            Assert.AreEqual(projects[0], projects[0].Tasks.First().Project);
            Assert.AreEqual(projects[0], projects[0].Tasks.Last().Project);

            var firstTask = fromDb.Tasks.First();
            Assert.AreEqual(1, firstTask.Records.Count);
            Assert.AreEqual(firstTask, firstTask.Records.First().Task);
            Assert.AreEqual(start, firstTask.Records.First().Start);
            Assert.AreEqual(stop, firstTask.Records.First().Stop);

            var lastTask = fromDb.Tasks.Last();
            Assert.AreEqual(3, lastTask.Records.Count);
            Assert.AreEqual(lastTask, lastTask.Records.First().Task);
        }

        private IList<RecordVM> records = new List<RecordVM>();
        private IList<TaskVM> tasks = new List<TaskVM>();
        private IList<ProjectVM> projects = new List<ProjectVM>();
        private DateTime start = new DateTime(2017, 10, 17, 8, 16, 16);
        private DateTime stop = new DateTime(2017, 10, 17, 9, 31, 1);

        private static ProjectDataRepository repository;
    }
}
