
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
                _newTaskName = value;
                RaisePropertyChanged(() => IsTaskAddingEnabled);
            }
        }

        public string NewTaskDuration
        {
            get => _newTaskDuration;
            set
            {
                _newTaskDuration = value;
                RaisePropertyChanged(() => IsTaskAddingEnabled);
            }
        }

        public ICommand AddNewTaskCommand => new RelayCommand(() =>
        {
            Tasks.Add(new TaskItemViewModel(NewTaskName, int.Parse(NewTaskDuration)));
        });

        public ICommand RemoveTaskCommand => new RelayCommand<TaskItemViewModel>(x =>
        {
            Tasks.Remove(x);
        });
                
        public string this[string propertyName] => TasksCommanderViewModelValidation.ValidateProperty(this, propertyName);

        public bool IsTaskAddingEnabled =>
            !string.IsNullOrWhiteSpace(NewTaskName) &&
            !string.IsNullOrWhiteSpace(NewTaskDuration) &&
            string.IsNullOrEmpty(TasksCommanderViewModelValidation.ValidateProperty(this, nameof(NewTaskName))) &&
            string.IsNullOrEmpty(TasksCommanderViewModelValidation.ValidateProperty(this, nameof(NewTaskDuration)));

        public string Error
        {
            get { return ""; }
        }

        static class TasksCommanderViewModelValidation
        {
            public static string ValidateProperty(TasksCommanderViewModel viewModel, string propertyName)
            {
                string error = string.Empty;
                switch (propertyName)
                {
                    case nameof(viewModel.NewTaskName):
                        if (string.IsNullOrEmpty(viewModel.NewTaskName))
                            break;

                        if (viewModel.NewTaskName.Length == 1 && char.IsWhiteSpace(viewModel.NewTaskName, 0))
                            error = "Название задачи должно быть не пустым";
                        break;

                    case nameof(viewModel.NewTaskDuration):

                        if (string.IsNullOrEmpty(viewModel.NewTaskDuration))
                            break;

                        if (!int.TryParse(viewModel.NewTaskDuration, out var duration))
                            error = "Длительность задачи должна быть указана целым числом";
                        else if (duration <= 0)
                            error = "Длительность задачи должна быть указана положительным целым числом";
                        break;
                }
                return error;
            }
        }

    }
}