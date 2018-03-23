using System;
using System.Globalization;
using System.Windows.Data;
using TaskListCommander.ViewModel;

namespace TaskListCommander.Converters
{
    class TaskNameAndStateToDescriptionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            switch (values[1])
            {
                case State.Completed:
                    return $"Задача {values[0]} - Выполнена";
                case State.Runnning:
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
