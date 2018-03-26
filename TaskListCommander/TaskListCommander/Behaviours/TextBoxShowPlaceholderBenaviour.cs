using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace TaskListCommander.AttachedProperties
{   
    public static class AdditionalContent
    {
        public static readonly DependencyProperty RightContentProperty =
    DependencyProperty.RegisterAttached("RightContent", typeof(UIElement), typeof(AdditionalContent),
        new UIPropertyMetadata(null, SetContent));

        private static void SetContent(object sender, DependencyPropertyChangedEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb != null)
            {
                var p = (tb.Parent as System.Windows.Controls.Panel).Children;
                var ind = (tb.Parent as System.Windows.Controls.Panel).Children.IndexOf(tb);
                var grid = new Grid();
                (tb.Parent as System.Windows.Controls.Panel).Children.Remove(tb);

                grid.Children.Add(tb);
                grid.Children.Add(new Button()
                {
                    Content = "B",
                    Width = 30,
                    Margin = new Thickness(4),
                    HorizontalAlignment = HorizontalAlignment.Right
                });
                p.Insert(ind, grid);
                //if (string.IsNullOrEmpty(tb.Text))
                //{
                //    tb.Text = GetPlaceholder(tb);
                //}
            }
        }

        public static void SetRightContent(UIElement element, UIElement value)
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


    public static class TextBoxExtended
    {
        public static readonly DependencyProperty PlaceholderProperty =
    DependencyProperty.RegisterAttached("Placeholder", typeof(string), typeof(TextBoxExtended),
        new UIPropertyMetadata(null, SetContent));

        private static void SetContent(object sender, DependencyPropertyChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                var parent = (textBox.Parent as Panel);
                var indexOfSender = parent.Children.IndexOf(textBox);
                parent.Children.Remove(textBox);

                var grid = new Grid()
                {

                };

                grid.Children.Add(textBox);
                var placeHolder = CreatePlaceHolderUiElement(textBox, e.NewValue.ToString());
                grid.Children.Add(placeHolder);
                parent.Children.Insert(indexOfSender, grid);

                placeHolder.MouseDown += delegate
                {
                    textBox.Focus();
                };
                textBox.GotFocus += delegate
                {
                    placeHolder.Visibility = Visibility.Collapsed;
                };
                textBox.LostFocus += delegate
                {
                    if (string.IsNullOrEmpty(textBox.Text))
                        placeHolder.Visibility = Visibility.Visible;
                };
            }
        }

        private static UIElement CreatePlaceHolderUiElement(TextBox textBox, string placeHolderText)
        {
            return new TextBlock()
            {
                Text = placeHolderText,
                Foreground = new SolidColorBrush(Colors.LightGray),
                HorizontalAlignment = textBox.HorizontalContentAlignment,
                VerticalAlignment = textBox.VerticalContentAlignment,
                TextAlignment = textBox.TextAlignment,
                Visibility = string.IsNullOrEmpty(textBox.Text) ? Visibility.Visible : Visibility.Collapsed
            };
        }

        public static void SetPlaceholder(UIElement element, UIElement value)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            element.SetValue(PlaceholderProperty, value);
        }

        public static UIElement GetPlaceholder(UIElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            return ((UIElement)element.GetValue(PlaceholderProperty));
        }
    }
}

