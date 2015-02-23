using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LynLogger.Views
{
    public class NoSizeDecorator : Decorator
    {
        public bool IgnoreChildWidthRequest { get; set; }
        public bool IgnoreChildHeightRequest { get; set; }

        private bool reMeasure = false;
        private Size lastSize;

        protected override Size MeasureOverride(Size constraint)
        {
            Size c = constraint;
            if(IgnoreChildHeightRequest) {
                c.Height = reMeasure ? lastSize.Height : 1;
            }
            if(IgnoreChildWidthRequest) {
                c.Width = reMeasure ? lastSize.Width : 1;
            }
            Child.Measure(c);
            reMeasure = false;

            return Child.DesiredSize;
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            lastSize = arrangeSize;
            reMeasure = true;

            Child.Measure(arrangeSize);
            Child.Arrange(new Rect(arrangeSize));
            return base.ArrangeOverride(new Size(Child.RenderSize.Width, Child.RenderSize.Height));
        }
    }
}
