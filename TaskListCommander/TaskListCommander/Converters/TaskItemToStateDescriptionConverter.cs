using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TaskListCommander.ViewModel;

namespace TaskListCommander.Converters
{
    class TaskItemToStateDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var task = value as TaskItemViewModel;
            var state = string.Empty;

            switch (task.State)
            {
                case State.Runnning:
                    break;
                case State.Completed:
                    state = " - Выполнена";
                    break;
            }

            return $"Задача {task.Name}{state}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
