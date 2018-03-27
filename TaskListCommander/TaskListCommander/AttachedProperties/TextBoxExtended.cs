using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TaskListCommander.AttachedProperties
{
    public static class TextBoxExtended
    {
        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.RegisterAttached("Placeholder", typeof(string), typeof(TextBoxExtended),
            new UIPropertyMetadata(null, SetContent));

        private static void SetContent(object sender, DependencyPropertyChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                var parent = (textBox.Parent as Panel);
                var indexOfSender = parent.Children.IndexOf(textBox);
                parent.Children.Remove(textBox);

                var grid = new Grid();

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
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
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