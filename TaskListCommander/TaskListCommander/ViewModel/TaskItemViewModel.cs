using System;
using GalaSoft.MvvmLight;
using System.Threading;
using System.Threading.Tasks;

namespace TaskListCommander.ViewModel
{
    public enum State
    {
        Runnning,
        Completed
    }

    public class TaskItemViewModel : ViewModelBase
    {
        private readonly int _durationMilliseconds;
        private int _progress;
        private State _state;
        private bool _canBeRemoved;

        public TaskItemViewModel(string name, int duration)
        {
            Name = name;
            _durationMilliseconds = duration * 1000;
            Start();
        }

        void Start()
        {
            var progressReporter = new Progress(amount => Progress = amount);

            State = State.Runnning;
            Progress = 0;

            Task.Run(() =>
            {
                WorkImitaion.Run(_durationMilliseconds, progressReporter);

                CanBeRemoved = true;
                State = State.Completed;
            });
        }

        public string Name { get; }

        public int Progress
        {
            get => _progress;
            set => Set(nameof(Progress), ref _progress, value);
        }

        public State State
        {
            get => _state;
            set => Set(nameof(State), ref _state, value);
        }

        public bool CanBeRemoved
        {
            get => _canBeRemoved;
            set => Set(nameof(CanBeRemoved), ref _canBeRemoved, value);
        }
    }

    public class Progress : IProgress<int>
    {
        private readonly Action<int> _callback;

        public Progress(Action<int> callback)
        {
            _callback = callback;
        }

        public void Report(int value)
        {
            _callback.Invoke(value);
        }
    }

    static class WorkImitaion
    {
        public static void Run(int durationMilliseconds, IProgress<int> progressReporter)
        {
            var stepsCount = 100;
            var timeStep = durationMilliseconds / stepsCount;
            var step = 100f / stepsCount;
            var progress = 0f;
            while (progress < 100)
            {
                Thread.Sleep(timeStep);
                progress += step;
                progressReporter.Report((int)progress);
            }
        }
    }
}