using GalaSoft.MvvmLight;
using System;
using System.Threading.Tasks;
using TaskListCommander.Model;

namespace TaskListCommander.ViewModel
{
    public class TaskItemViewModel : ViewModelBase
    {
        private readonly TimeSpan _duration;
        private readonly TaskModel _model;
        private int _progress;
        private TaskState _state;
        private bool _canBeRemoved;

        public TaskItemViewModel(string name, TimeSpan duration)
        {
            Name = name;
            _duration = duration;
            _model = new TaskModel(duration);
            _model.ProgressChanged += _model_ProgressChanged;
            Start();
        }

        private void _model_ProgressChanged(object sender, ProgressInfo e)
        {
            State = e.State;
            Progress = e.Percents;
        }

        void Start()
        {
            Task.Run(() =>
            {
                _model.Run();

                CanBeRemoved = true;
            });
        }

        public string Name { get; }

        public int Progress
        {
            get => _progress;
            set => Set(nameof(Progress), ref _progress, value);
        }

        public TaskState State
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
}