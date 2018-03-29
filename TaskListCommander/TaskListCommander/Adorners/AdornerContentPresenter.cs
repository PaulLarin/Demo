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
        private VisualCollection _Visuals;
        private ContentPresenter _ContentPresenter;
        private readonly FrameworkElement adornedElement;

        public AdornerContentPresenter(FrameworkElement adornedElement)
          : base(adornedElement)
        {
            _Visuals = new VisualCollection(this);
            _ContentPresenter = new ContentPresenter();
            _Visuals.Add(_ContentPresenter);
        }

        public AdornerContentPresenter(FrameworkElement adornedElement, FrameworkElement content)
          : this(adornedElement)
        {
            this.adornedElement = adornedElement;
            Content = content;
            Content.DataContext = adornedElement.DataContext;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            _ContentPresenter.Measure(constraint);
            return _ContentPresenter.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _ContentPresenter.Arrange(new Rect(0, 0,
                 adornedElement.ActualWidth, adornedElement.ActualHeight));
            return _ContentPresenter.RenderSize;
        }

        protected override Visual GetVisualChild(int index)
        { return _Visuals[index]; }

        protected override int VisualChildrenCount
        { get { return _Visuals.Count; } }

        public FrameworkElement Content
        {
            get { return (FrameworkElement)_ContentPresenter.Content; }
            set { _ContentPresenter.Content = value; }
        }
    }
}
