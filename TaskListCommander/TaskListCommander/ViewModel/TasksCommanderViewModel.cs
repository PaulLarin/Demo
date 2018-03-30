
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Input;

namespace TaskListCommander.ViewModel
{
    public class TasksCommanderViewModel : ViewModelBase, IDataErrorInfo
    {
        private string _newTaskDuration;
        private string _newTaskName;
        private static List<TaskItemViewModel> _tasksList = new List<TaskItemViewModel>();

        public ObservableCollection<TaskItemViewModel> Tasks { get; } = new ObservableCollection<TaskItemViewModel>();

        public string NewTaskName
        {
            get => _newTaskName;
            set
            {
                if (Set(() => NewTaskName, ref _newTaskName, value))
                    RaisePropertyChanged(() => IsTaskAddingEnabled);
            }
        }

        public string NewTaskDuration
        {
            get => _newTaskDuration;
            set
            {
                if (Set(() => NewTaskDuration, ref _newTaskDuration, value))
                    RaisePropertyChanged(() => IsTaskAddingEnabled);
            }
        }

        readonly Random _random = new Random();

        public ICommand AddNewTaskCommand => new RelayCommand(() =>
        {
            var duration = TimeSpan.FromSeconds(int.Parse(NewTaskDuration));

            Tasks.Add(new TaskItemViewModel(NewTaskName, duration));
        });

        public ICommand RemoveTaskCommand => new RelayCommand<TaskItemViewModel>(x =>
        {
            Tasks.Remove(x);
        });

        public ICommand GenerateRandomTaskNameCommand => new RelayCommand(() =>
        {
            NewTaskName = _random.Next(1000, 9999).ToString();
        });

        public ICommand GenerateRandomTaskDurationCommand => new RelayCommand(() =>
        {
            NewTaskDuration = _random.Next(30).ToString();
        });

        public string this[string propertyName]
        {
            get
            {
                switch (propertyName)
                {
                    case nameof(NewTaskName):
                        return ValidateNewTaskName();

                    case nameof(NewTaskDuration):
                        return ValidateNewTaskDuration();
                }

                return null;
            }
        }

        public bool IsTaskAddingEnabled =>
            !string.IsNullOrWhiteSpace(NewTaskName) &&
            !string.IsNullOrWhiteSpace(NewTaskDuration) &&
            string.IsNullOrEmpty(ValidateNewTaskDuration()) &&
            string.IsNullOrEmpty(ValidateNewTaskName());

        public string Error => null;

        string ValidateNewTaskName()
        {
            string error = null;

            if (string.IsNullOrEmpty(NewTaskName))
                return error;

            if (NewTaskName.Length == 1 && char.IsWhiteSpace(NewTaskName, 0))
                error = "Название задачи должно быть не пустым";

            return error;
        }

        string ValidateNewTaskDuration()
        {
            string error = null;


            if (string.IsNullOrEmpty(NewTaskDuration))
                return error;

            if (!int.TryParse(NewTaskDuration, out var duration))
                error = "Длительность задачи должна быть указана целым числом";
            else if (duration <= 0)
                error = "Длительность задачи должна быть указана положительным целым числом";

            return error;
        }
    }
}