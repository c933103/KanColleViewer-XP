using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Grabacr07.KanColleViewer.Views.Controls
{
    public class NoSizeDecorator : Decorator
    {
        public bool IgnoreWidth { get; set; }
        public bool IgnoreHeight { get; set; }

        private Size lastArrange;

        protected override Size MeasureOverride(Size constraint)
        {
            if (IgnoreHeight) constraint.Height = lastArrange.Height;
            if (IgnoreWidth) constraint.Width = lastArrange.Width;
            Child.Measure(constraint);
            var request = Child.DesiredSize;
            if (IgnoreHeight) request.Height = 0;
            if (IgnoreWidth) request.Width = 0;
            return request;
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            lastArrange = arrangeSize;
            Child.Measure(arrangeSize);
            Child.Arrange(new Rect(arrangeSize));
            return arrangeSize;
        }
    }
}
