using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Tests.Utilities;
using TimeTracker.DbMappings;
using TimeTracker.TimeTracking;
using TimeTracker.ViewModels;

namespace Tests
{
    [TestClass]
    public class RecordsViewModelTests
    {
        [TestMethod]
        public void PauseFromSessionLockCommand_startAfterLock_stopAfterUnlock()
        {
            var repoUtility = new RepositoryTools();

            repoUtility.InRepository(repository)
                .InProject("Project 1")
                .InTask("Task 1")
                .AddRecord(
                    new DateTime(2018, 3, 17, 10, 00, 00),
                    new DateTime(2018, 3, 17, 11, 00, 00));

            var record = CreateLockRecord(
                new DateTime(2018, 3, 17, 10, 10, 00),
                new DateTime(2018, 3, 17, 10, 20, 00));

            model.SelectedDate = new DateTime(2018, 3, 17);
            model.CurrentLoggerRecord = record;
            model.PauseFromSessionLockCommand.Execute(null);

            var resultRecords = repository.GetRecords(new DateTime(2018, 3, 17));
            Assert.AreEqual(2, resultRecords.Count);
            Assert.AreEqual(new DateTime(2018, 3, 17, 10, 00, 00), resultRecords[0].Start);
            Assert.AreEqual(new DateTime(2018, 3, 17, 10, 10, 00), resultRecords[0].Stop);
            Assert.AreEqual(new DateTime(2018, 3, 17, 10, 20, 00), resultRecords[1].Start);
            Assert.AreEqual(new DateTime(2018, 3, 17, 11, 00, 00), resultRecords[1].Stop);
        }

        [TestMethod]
        public void PauseFromSessionLockCommand_startOnLock_stopAfterUnlock()
        {
            var repoUtility = new RepositoryTools();

            repoUtility.InRepository(repository)
                .InProject("Project 1")
                .InTask("Task 1")
                .AddRecord(
                    new DateTime(2018, 3, 17, 10, 00, 00),
                    new DateTime(2018, 3, 17, 11, 00, 00));

            var record = CreateLockRecord(
                new DateTime(2018, 3, 17, 10, 00, 00),
                new DateTime(2018, 3, 17, 10, 20, 00));

            model.SelectedDate = new DateTime(2018, 3, 17);
            model.CurrentLoggerRecord = record;
            model.PauseFromSessionLockCommand.Execute(null);

            var resultRecords = repository.GetRecords(new DateTime(2018, 3, 17));
            Assert.AreEqual(1, resultRecords.Count);
            Assert.AreEqual(new DateTime(2018, 3, 17, 10, 20, 00), resultRecords[0].Start);
            Assert.AreEqual(new DateTime(2018, 3, 17, 11, 00, 00), resultRecords[0].Stop);
        }

        [TestMethod]
        public void PauseFromSessionLockCommand_startAfterLock_stopOnUnlock()
        {
            var repoUtility = new RepositoryTools();

            repoUtility.InRepository(repository)
                .InProject("Project 1")
                .InTask("Task 1")
                .AddRecord(
                    new DateTime(2018, 3, 17, 10, 00, 00),
                    new DateTime(2018, 3, 17, 11, 00, 00));

            var record = CreateLockRecord(
                new DateTime(2018, 3, 17, 10, 10, 00),
                new DateTime(2018, 3, 17, 11, 00, 00));

            model.SelectedDate = new DateTime(2018, 3, 17);
            model.CurrentLoggerRecord = record;
            model.PauseFromSessionLockCommand.Execute(null);

            var resultRecords = repository.GetRecords(new DateTime(2018, 3, 17));
            Assert.AreEqual(1, resultRecords.Count);
            Assert.AreEqual(new DateTime(2018, 3, 17, 10, 00, 00), resultRecords[0].Start);
            Assert.AreEqual(new DateTime(2018, 3, 17, 10, 10, 00), resultRecords[0].Stop);
        }

        [TestMethod]
        public void PauseFromSessionLockCommand_startOnLock_stopOnUnlock()
        {
            var repoUtility = new RepositoryTools();

            repoUtility.InRepository(repository)
                .InProject("Project 1")
                .InTask("Task 1")
                .AddRecord(
                    new DateTime(2018, 3, 17, 10, 00, 00),
                    new DateTime(2018, 3, 17, 11, 00, 00));

            var record = CreateLockRecord(
                new DateTime(2018, 3, 17, 10, 00, 00),
                new DateTime(2018, 3, 17, 11, 00, 00));

            model.SelectedDate = new DateTime(2018, 3, 17);
            model.CurrentLoggerRecord = record;
            model.PauseFromSessionLockCommand.Execute(null);

            var resultRecords = repository.GetRecords(new DateTime(2018, 3, 17));
            Assert.AreEqual(0, resultRecords.Count);
        }

        [TestMethod]
        public void RecordFromSessionLockCommand_startAfterLock_stopAfterUnlock()
        {
            var repoUtility = new RepositoryTools();

            repoUtility.InRepository(repository)
                .InProject("Project 1")
                .AddTask("Task 1")
                .InTask("Task 2")
                .AddRecord(
                    new DateTime(2018, 3, 17, 10, 00, 00),
                    new DateTime(2018, 3, 17, 11, 00, 00));

            var record = CreateLockRecord(
                new DateTime(2018, 3, 17, 10, 10, 00),
                new DateTime(2018, 3, 17, 10, 20, 00));

            model.SelectedDate = new DateTime(2018, 3, 17);
            model.CurrentLoggerRecord = record;
            model.RecordFromSessionLockCommand.Execute(null);

            var resultRecords = repository.GetRecords(new DateTime(2018, 3, 17));
            Assert.AreEqual(3, resultRecords.Count);
            Assert.AreEqual("Task 2", resultRecords[0].Task.Name);
            Assert.AreEqual(new DateTime(2018, 3, 17, 10, 00, 00), resultRecords[0].Start);
            Assert.AreEqual(new DateTime(2018, 3, 17, 10, 10, 00), resultRecords[0].Stop);
            Assert.AreEqual("Task 1", resultRecords[1].Task.Name);
            Assert.AreEqual(new DateTime(2018, 3, 17, 10, 10, 00), resultRecords[1].Start);
            Assert.AreEqual(new DateTime(2018, 3, 17, 10, 20, 00), resultRecords[1].Stop);
            Assert.AreEqual("Task 2", resultRecords[2].Task.Name);
            Assert.AreEqual(new DateTime(2018, 3, 17, 10, 20, 00), resultRecords[2].Start);
            Assert.AreEqual(new DateTime(2018, 3, 17, 11, 00, 00), resultRecords[2].Stop);
        }

        [TestMethod]
        public void RecordFromSessionLockCommand_startOnLock_stopAfterUnlock()
        {
            var repoUtility = new RepositoryTools();

            repoUtility.InRepository(repository)
                .InProject("Project 1")
                .AddTask("Task 1")
                .InTask("Task 2")
                .AddRecord(
                    new DateTime(2018, 3, 17, 10, 00, 00),
                    new DateTime(2018, 3, 17, 11, 00, 00));

            var record = CreateLockRecord(
                new DateTime(2018, 3, 17, 10, 00, 00),
                new DateTime(2018, 3, 17, 10, 20, 00));

            model.SelectedDate = new DateTime(2018, 3, 17);
            model.CurrentLoggerRecord = record;
            model.RecordFromSessionLockCommand.Execute(null);

            var resultRecords = repository
                .GetRecords(new DateTime(2018, 3, 17))
                .OrderBy(r => r.Stop)
                .ToList();

            Assert.AreEqual(2, resultRecords.Count);
            Assert.AreEqual("Task 1", resultRecords[0].Task.Name);
            Assert.AreEqual(new DateTime(2018, 3, 17, 10, 00, 00), resultRecords[0].Start);
            Assert.AreEqual(new DateTime(2018, 3, 17, 10, 20, 00), resultRecords[0].Stop);
            Assert.AreEqual("Task 2", resultRecords[1].Task.Name);
            Assert.AreEqual(new DateTime(2018, 3, 17, 10, 20, 00), resultRecords[1].Start);
            Assert.AreEqual(new DateTime(2018, 3, 17, 11, 00, 00), resultRecords[1].Stop);
        }

        [TestMethod]
        public void RecordFromSessionLockCommand_startAfterLock_stopOnUnlock()
        {
            var repoUtility = new RepositoryTools();

            repoUtility.InRepository(repository)
                .InProject("Project 1")
                .AddTask("Task 1")
                .InTask("Task 2")
                .AddRecord(
                    new DateTime(2018, 3, 17, 10, 00, 00),
                    new DateTime(2018, 3, 17, 11, 00, 00));

            var record = CreateLockRecord(
                new DateTime(2018, 3, 17, 10, 10, 00),
                new DateTime(2018, 3, 17, 11, 00, 00));

            model.SelectedDate = new DateTime(2018, 3, 17);
            model.CurrentLoggerRecord = record;
            model.RecordFromSessionLockCommand.Execute(null);

            var resultRecords = repository
                .GetRecords(new DateTime(2018, 3, 17))
                .OrderBy(r => r.Stop)
                .ToList();

            Assert.AreEqual(2, resultRecords.Count);
            Assert.AreEqual("Task 2", resultRecords[0].Task.Name);
            Assert.AreEqual(new DateTime(2018, 3, 17, 10, 00, 00), resultRecords[0].Start);
            Assert.AreEqual(new DateTime(2018, 3, 17, 10, 10, 00), resultRecords[0].Stop);
            Assert.AreEqual("Task 1", resultRecords[1].Task.Name);
            Assert.AreEqual(new DateTime(2018, 3, 17, 10, 10, 00), resultRecords[1].Start);
            Assert.AreEqual(new DateTime(2018, 3, 17, 11, 00, 00), resultRecords[1].Stop);
        }

        [TestMethod]
        public void RecordFromSessionLockCommand_startOnLock_stopOnUnlock()
        {
            var repoUtility = new RepositoryTools();

            repoUtility.InRepository(repository)
                .InProject("Project 1")
                .AddTask("Task 1")
                .InTask("Task 2")
                .AddRecord(
                    new DateTime(2018, 3, 17, 10, 00, 00),
                    new DateTime(2018, 3, 17, 11, 00, 00));

            var record = CreateLockRecord(
                new DateTime(2018, 3, 17, 10, 00, 00),
                new DateTime(2018, 3, 17, 11, 00, 00));

            model.SelectedDate = new DateTime(2018, 3, 17);
            model.CurrentLoggerRecord = record;
            model.RecordFromSessionLockCommand.Execute(null);

            var resultRecords = repository
                .GetRecords(new DateTime(2018, 3, 17))
                .OrderBy(r => r.Stop)
                .ToList();

            Assert.AreEqual(1, resultRecords.Count);
            Assert.AreEqual("Task 1", resultRecords[0].Task.Name);
            Assert.AreEqual(new DateTime(2018, 3, 17, 10, 00, 00), resultRecords[0].Start);
            Assert.AreEqual(new DateTime(2018, 3, 17, 11, 00, 00), resultRecords[0].Stop);
        }

        [TestInitialize]
        public void TestInit()
        {
            repository = new ProjectDataRepository(true);
            mockLogger = new Mock<WindowsSessionLogger>();

            var records = new ObservableCollection<SessionLockRecord>();
            mockLogger.SetupProperty(p => p.Records, records);

            model = new RecordsViewModel(repository, mockLogger.Object);
        }

        private SessionLockRecord CreateLockRecord(DateTime lockTime, DateTime unlockTime)
        {
            var record = new SessionLockRecord
            {
                LockTime = lockTime,
                UnlockTime = unlockTime
            };

            mockLogger.Object.Records.Add(record);
            return record;
        }

        private IProjectDataRepository repository;
        private Mock<WindowsSessionLogger> mockLogger;
        private RecordsViewModel model;
    }
}
