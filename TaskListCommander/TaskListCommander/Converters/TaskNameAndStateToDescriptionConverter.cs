using System;
using System.Globalization;
using System.Windows.Data;
using TaskListCommander.ViewModel;
using TaskListCommander.Model;

namespace TaskListCommander.Converters
{
    class TaskNameAndStateToDescriptionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            switch (values[1])
            {
                case TaskState.Completed:
                    return $"Задача {values[0]} - Выполнена";
                case TaskState.Runnning:
                default:
                    return $"Задача {values[0]}";
            }

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
