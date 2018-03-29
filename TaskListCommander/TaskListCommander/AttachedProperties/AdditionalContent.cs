using System;
using System.Windows;
using System.Windows.Documents;
using TaskListCommander.Adorners;

namespace TaskListCommander.AttachedProperties
{
    public static class AdditionalContent
    {
        public static readonly DependencyProperty RightContentProperty =
    DependencyProperty.RegisterAttached("RightContent", typeof(FrameworkElement), typeof(AdditionalContent),
        new PropertyMetadata(null, SetContent));

        private static void SetContent(object sender, DependencyPropertyChangedEventArgs e)
        {
            var element = sender as FrameworkElement;
            if (element != null)
            {
                element.Loaded += delegate
                {
                    var content = e.NewValue as FrameworkElement;

                    content.HorizontalAlignment = HorizontalAlignment.Right;
                    content.Margin = new Thickness(4);

                    var adorner = new AdornerContentPresenter(element, content);
                    AdornerLayer.GetAdornerLayer(element).Add(adorner);
                };
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

