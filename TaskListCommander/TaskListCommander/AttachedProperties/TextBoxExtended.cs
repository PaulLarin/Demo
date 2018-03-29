using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using TaskListCommander.Adorners;

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
                textBox.Loaded += delegate
                {
                    var placeHolder = CreatePlaceHolderUIElement(textBox, e.NewValue.ToString());

                    placeHolder.MouseDown += delegate
                    {
                        textBox.Focus();
                    };

                    textBox.TextChanged += delegate
                    {
                        if (string.IsNullOrEmpty(textBox.Text))
                            placeHolder.Visibility = Visibility.Visible;
                        else
                            placeHolder.Visibility = Visibility.Collapsed;
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

                    var adorner = new AdornerContentPresenter(textBox, placeHolder);
                    AdornerLayer.GetAdornerLayer(textBox).Add(adorner);
                };
            }
        }

        private static FrameworkElement CreatePlaceHolderUIElement(TextBox textBox, string placeHolderText)
        {
            return new TextBlock()
            {
                Text = placeHolderText,
                Foreground = new SolidColorBrush(Colors.Gray),
                FontFamily = textBox.FontFamily,
                FontSize = textBox.FontSize,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = textBox.TextAlignment,
                Visibility = string.IsNullOrEmpty(textBox.Text) ? Visibility.Visible : Visibility.Collapsed
            };
        }

        public static void SetPlaceholder(UIElement element, string value)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            element.SetValue(PlaceholderProperty, value);
        }

        public static string GetPlaceholder(UIElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            return ((string)element.GetValue(PlaceholderProperty));
        }
    }
}