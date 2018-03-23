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
        public static string GetWatermarkText(DependencyObject obj)
        {
            return (string)obj.GetValue(WatermarkText);
        }

        public static void SetWatermarkText(DependencyObject obj, string value)
        {
            obj.SetValue(WatermarkText, value);
        }

      
      
        public static readonly DependencyProperty WatermarkText =
            DependencyProperty.RegisterAttached("WatermarkText",
            typeof(string), typeof(TextBoxShowPlaceholderBenaviour),
            new UIPropertyMetadata(string.Empty, OnWatermarkTextChanged));

        private static void OnWatermarkTextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb != null)
            {
                tb.Text = (string)e.NewValue;
                tb.GotFocus += OnInputTextBoxGotFocus;
                tb.LostFocus += OnInputTextBoxLostFocus;
                if (string.IsNullOrEmpty(tb.Text))
                    tb.Text = GetWatermarkText(tb);
            }
        }

      

        private static void OnInputTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            var tb = e.OriginalSource as TextBox;
            if (tb != null)
            {
                if (string.IsNullOrEmpty(tb.Text))
                    tb.Text = GetWatermarkText(tb);
            }
        }

        private static void OnInputTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            var tb = e.OriginalSource as TextBox;
            if (tb != null)
            {
                if (tb.Text == GetWatermarkText(tb))
                    tb.Text = string.Empty;
            }
        }
    }
}

