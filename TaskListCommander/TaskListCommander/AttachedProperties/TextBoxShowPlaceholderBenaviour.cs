using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace TaskListCommander.AttachedProperties
{   
    public static class AdditionalContent
    {
        public static readonly DependencyProperty RightContentProperty =
    DependencyProperty.RegisterAttached("RightContent", typeof(FrameworkElement), typeof(AdditionalContent),
        new UIPropertyMetadata(null, SetContent));

        private static void SetContent(object sender, DependencyPropertyChangedEventArgs e)
        {
            var element = sender as FrameworkElement;
            if (element != null)
            {
                var parent = element.Parent as Panel;
                var indexOfSender = parent.Children.IndexOf(element);
                parent.Children.Remove(element);

                var grid = new Grid();

                grid.Children.Add(element);
                var content = e.NewValue as FrameworkElement;
                content.HorizontalAlignment = HorizontalAlignment.Right;
                content.Margin = new Thickness(4);
                grid.Children.Add(content);
                parent.Children.Insert(indexOfSender, grid);
            }
        }

        public static void SetRightContent(UIElement element, FrameworkElement value)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            element.SetValue(RightContentProperty, value);
        }

        public static UIElement GetRightContent(UIElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            return ((UIElement)element.GetValue(RightContentProperty));
        }
    }
}

