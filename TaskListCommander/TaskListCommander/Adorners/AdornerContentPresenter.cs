using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace TaskListCommander.Adorners
{

    public class AdornerContentPresenter : Adorner
    {
        private readonly VisualCollection _visuals;
        private readonly ContentPresenter _contentPresenter;
        private readonly FrameworkElement _adornedElement;

        public AdornerContentPresenter(FrameworkElement adornedElement)
          : base(adornedElement)
        {
            _visuals = new VisualCollection(this);
            _contentPresenter = new ContentPresenter();
            _visuals.Add(_contentPresenter);
        }

        public AdornerContentPresenter(FrameworkElement adornedElement, FrameworkElement content)
          : this(adornedElement)
        {
            _adornedElement = adornedElement;
            Content = content;
            Content.DataContext = adornedElement.DataContext;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            _contentPresenter.Measure(constraint);
            return _contentPresenter.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _contentPresenter.Arrange(new Rect(0, 0,
                 _adornedElement.ActualWidth, _adornedElement.ActualHeight));
            return _contentPresenter.RenderSize;
        }

        protected override Visual GetVisualChild(int index) => _visuals[index];

        protected override int VisualChildrenCount => _visuals.Count;

        public FrameworkElement Content
        {
            get => (FrameworkElement)_contentPresenter.Content;
            set => _contentPresenter.Content = value;
        }
    }
}
