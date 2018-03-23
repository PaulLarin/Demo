using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TaskListCommander.Behaviours
{
    public static class TextBoxShowPlaceholderBenaviour
    {
        public static string GetPlaceholder(DependencyObject obj)
        {
            return (string)obj.GetValue(Placeholder);
        }

        public static void SetPlaceholder(DependencyObject obj, string value)
        {
            obj.SetValue(Placeholder, value);
        }
      
        public static readonly DependencyProperty Placeholder =
            DependencyProperty.RegisterAttached("Placeholder",
            typeof(string), typeof(TextBoxShowPlaceholderBenaviour),
            new UIPropertyMetadata(string.Empty, OnPlaceholderChanged));

        private static void OnPlaceholderChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb != null)
            {
                tb.Text = (string)e.NewValue;
                tb.GotFocus += OnInputTextBoxGotFocus;
                tb.LostFocus += OnInputTextBoxLostFocus;
                if (string.IsNullOrEmpty(tb.Text))
                    tb.Text = GetPlaceholder(tb);
            }
        }

      

        private static void OnInputTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            var tb = e.OriginalSource as TextBox;
            if (tb != null)
            {
                if (string.IsNullOrEmpty(tb.Text))
                {
                    tb.Text = GetPlaceholder(tb);
                }
            }
        }

        private static void OnInputTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            var tb = e.OriginalSource as TextBox;
            if (tb != null)
            {
                if (tb.Text == GetPlaceholder(tb))
                    tb.Text = string.Empty;
            }
        }
    }
}

