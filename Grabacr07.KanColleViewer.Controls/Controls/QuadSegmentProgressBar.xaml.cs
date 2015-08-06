using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Grabacr07.KanColleViewer.Controls
{
    /// <summary>
    /// QuadSegmentProgressBar.xaml 的交互逻辑
    /// </summary>
    public partial class QuadSegmentProgressBar : UserControl
    {
        public static readonly DependencyProperty dpValue = DependencyProperty.Register(nameof(Value), typeof(int), typeof(QuadSegmentProgressBar), new PropertyMetadata(0, AdjustValueWidth));
        public static readonly DependencyProperty dpMaximum = DependencyProperty.Register(nameof(Maximum), typeof(int), typeof(QuadSegmentProgressBar), new PropertyMetadata(0, AdjustValueWidth));

        public int Value
        {
            get { return (int)GetValue(dpValue); }
            set { SetValue(dpValue, value); }
        }

        public int Maximum
        {
            get { return (int)GetValue(dpMaximum); }
            set { SetValue(dpMaximum, value); }
        }

        public Brush Segment1Brush { get; set; } = Brushes.Red;
        public Brush Segment2Brush { get; set; } = Brushes.Orange;
        public Brush Segment3Brush { get; set; } = Brushes.Yellow;
        public Brush Segment4Brush { get; set; } = Brushes.Green;

        private double _threshold1 = 0.25;
        private double _threshold2 = 0.5;
        private double _threshold3 = 0.75;

        public double Threshold1
        {
            get { return _threshold1; }
            set
            {
                if (value < 0 || value > 1) throw new ArgumentException();
                _threshold1 = value;
                AdjustSegmentWidth();
            }
        }

        public double Threshold2
        {
            get { return _threshold2; }
            set
            {
                if (value < 0 || value > 1) throw new ArgumentException();
                _threshold2 = value;
                AdjustSegmentWidth();
            }
        }

        public double Threshold3
        {
            get { return _threshold3; }
            set
            {
                if (value < 0 || value > 1) throw new ArgumentException();
                _threshold3 = value;
                AdjustSegmentWidth();
            }
        }

        public QuadSegmentProgressBar()
        {
            InitializeComponent();
        }

        private void AdjustSegmentWidth()
        {
            if(_threshold1 > 0) {
                Segment1.Width = new GridLength(_threshold1, GridUnitType.Star);
            } else {
                Segment1.Width = new GridLength(0);
            }

            var previous = _threshold1;

            if (_threshold2 > previous) {
                Segment2.Width = new GridLength(_threshold2 - previous, GridUnitType.Star);
                previous = _threshold2;
            } else {
                Segment2.Width = new GridLength(0);
            }

            if(_threshold3 > previous) {
                Segment3.Width = new GridLength(_threshold3 - previous, GridUnitType.Star);
                previous = _threshold3;
            } else {
                Segment3.Width = new GridLength(0);
            }

            if(1 > previous) {
                Segment4.Width = new GridLength(1 - previous, GridUnitType.Star);
            } else {
                Segment4.Width = new GridLength(0);
            }
        }

        private static void AdjustValueWidth(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (QuadSegmentProgressBar)d;
            var valueSize = 1.0 * self.Value / self.Maximum;
            if (valueSize > 1) valueSize = 1;

            self.ValueSize.Width = new GridLength(valueSize, GridUnitType.Star);
            self.RemainderSize.Width = new GridLength(1 - valueSize, GridUnitType.Star);
        }
    }
}
