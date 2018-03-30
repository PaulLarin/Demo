using System;
using System.Threading;

namespace TaskListCommander.Model
{
    class TaskModel: Progress<ProgressInfo>
    {
        private readonly TimeSpan _duration;

        public TaskModel(TimeSpan duration)
        {
            _duration = duration;
        }

        public void Run()
        {
            OnReport(new ProgressInfo(TaskState.Runnning, 0));

            WorkImitaion();

            OnReport(new ProgressInfo(TaskState.Completed, 100));
        }

        private void WorkImitaion()
        {
            var stepsCount = 100;
            var timeStep = _duration.TotalMilliseconds / stepsCount;
            var step = 100f / stepsCount;
            var progress = 0f;
            while (progress < 100)
            {
                Thread.Sleep(Convert.ToInt32(timeStep));
                progress += step;
                OnReport(new ProgressInfo(TaskState.Runnning, (int)progress));
            }
        }
    }
}
