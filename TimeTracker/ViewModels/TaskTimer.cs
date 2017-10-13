using Prism.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows.Threading;

namespace TimeTracker.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class TaskTimer : BindableBase
    {
        public TaskTimer()
        {
            SetUpTimer();
        }

        private void SetUpTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.5);
            timer.Tick += UpdateElapsedTime;
            PropertyChanged += OnPropertyChanged;
        }

        private void UpdateElapsedTime(object sender, EventArgs e)
        {
            var diff = DateTime.Now - startTime;
            ElapsedTime = new TimeSpan(diff.Hours, diff.Minutes, diff.Seconds);
        }

        private void OnPropertyChanged(
            object sender, 
            PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(IsMeasuring))
                ToggleTimer();
        }

        private void ToggleTimer()
        {
            if (IsMeasuring)
                StartTimer();
            else
                StopTimer();
        }

        private void StartTimer()
        {
            Reset();
            timer.Start();
        }

        public void Reset()
        {
            ElapsedTime = new TimeSpan();
            StartTime = DateTime.Now;
        }

        private void StopTimer()
        {
            timer.Stop();
        }

        public bool IsMeasuring
        {
            get { return isMeasuring; }
            set { SetProperty(ref isMeasuring, value); }
        }

        public DateTime StartTime
        {
            get { return startTime; }
            set { SetProperty(ref startTime, value); }
        }

        public TimeSpan ElapsedTime
        {
            get { return elapsedTime; }
            set { SetProperty(ref elapsedTime, value); }
        }

        private DispatcherTimer timer;
        private bool isMeasuring;
        private DateTime startTime;
        private TimeSpan elapsedTime;
    }
}
