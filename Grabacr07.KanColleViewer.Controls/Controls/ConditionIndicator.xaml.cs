using Grabacr07.KanColleWrapper.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// ConditionIndicator.xaml 的交互逻辑
    /// </summary>
    public partial class ConditionIndicator : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty dpCondition = DependencyProperty.Register(nameof(Condition), typeof(int), typeof(ConditionIndicator), new PropertyMetadata(49, ConditionChanged));
        public static readonly DependencyProperty dpConditionType = DependencyProperty.Register(nameof(ConditionType), typeof(ConditionType), typeof(ConditionIndicator), new PropertyMetadata(ConditionType.Normal, ConditionTypeChanged));

        private static readonly Brush _brushBrilliant = new SolidColorBrush(Color.FromRgb(255, 255, 64));
        private static readonly Brush _brushTired = new SolidColorBrush(Color.FromRgb(255, 200, 128));
        private static readonly Brush _brushOrangeTired = new SolidColorBrush(Color.FromRgb(255, 128, 32));
        private static readonly Brush _brushRedTired = new SolidColorBrush(Color.FromRgb(255, 32, 32));

        static ConditionIndicator()
        {
            _brushBrilliant.Freeze();
            _brushOrangeTired.Freeze();
            _brushRedTired.Freeze();
            _brushTired.Freeze();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public int Condition
        {
            get { return (int)GetValue(dpCondition); }
            set { SetValue(dpCondition, value); }
        }

        public ConditionType ConditionType
        {
            get { return (ConditionType)GetValue(dpConditionType); }
            set { SetValue(dpConditionType, value); }
        }

        private Brush _conditionBrush = Brushes.White;
        public Brush ConditionBrush
        {
            get { return _conditionBrush; }
            set
            {
                if (value == _conditionBrush) return;
                _conditionBrush = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ConditionBrush)));
            }
        }

        private Brush _normBrush = Brushes.White;
        public Brush NormBrush
        {
            get { return _normBrush; }
            set
            {
                if (value == _normBrush) return;
                _normBrush = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NormBrush)));
            }
        }

        private Point _normArcEnd = new Point(15.00005,0);
        public Point NormArcEnd
        {
            get { return _normArcEnd; }
            set
            {
                if (value == _normArcEnd) return;
                _normArcEnd = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NormArcEnd)));
            }
        }

        private bool _normLargeArc = true;
        public bool NormLargeArc
        {
            get { return _normLargeArc; }
            set
            {
                if (value == _normLargeArc) return;
                _normLargeArc = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NormLargeArc)));
            }
        }

        private Point _kiraArcEnd = new Point(15, 0);
        public Point KiraArcEnd
        {
            get { return _kiraArcEnd; }
            set
            {
                if (value == _kiraArcEnd) return;
                _kiraArcEnd = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(KiraArcEnd)));
            }
        }

        private bool _kiraLargeArc = false;
        public bool KiraLargeArc
        {
            get { return _kiraLargeArc; }
            set
            {
                if (value == _kiraLargeArc) return;
                _kiraLargeArc = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(KiraLargeArc)));
            }
        }

        private Point _kiraKiraArcEnd = new Point(15, 0);
        public Point KiraKiraArcEnd
        {
            get { return _kiraKiraArcEnd; }
            set
            {
                if (value == _kiraKiraArcEnd) return;
                _kiraKiraArcEnd = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(KiraKiraArcEnd)));
            }
        }

        private bool _kiraKiraLargeArc = false;
        public bool KiraKiraLargeArc
        {
            get { return _kiraKiraLargeArc; }
            set
            {
                if (value == _kiraKiraLargeArc) return;
                _kiraKiraLargeArc = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(KiraKiraLargeArc)));
            }
        }

        private int _condFontSize = 14;
        public int CondFontSize
        {
            get { return _condFontSize; }
            set
            {
                if (value == _condFontSize) return;
                _condFontSize = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CondFontSize)));
            }
        }

        private static void ConditionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (ConditionIndicator)d;
            var cond = (int)e.NewValue;
            var norm = cond;
            var kira = cond - 49;
            var kirakira = cond - 85;
            if (norm > 49) norm = 49;
            if (kira < 0) kira = 0;
            if (kira > 36) kira = 36;
            if (kirakira < 0) kirakira = 0;

            var radNorm = (2 * Math.PI - 0.00001) / 50 * (norm + 1);
            var endNormX = 15 - 15 * Math.Sin(radNorm);
            var endNormY = 15 - 15 * Math.Cos(radNorm);
            self.NormArcEnd = new Point(endNormX, endNormY);
            self.NormLargeArc = radNorm >= Math.PI;

            var radKira = (2 * Math.PI - 0.00001) / 36 * kira;
            var endKiraX = 15 - 15 * Math.Sin(radKira);
            var endKiraY = 15 - 15 * Math.Cos(radKira);
            self.KiraArcEnd = new Point(endKiraX, endKiraY);
            self.KiraLargeArc = radKira >= Math.PI;

            var radKiraKira = (2 * Math.PI - 0.00001) / 15 * kirakira;
            var endKiraKiraX = 15 - 15 * Math.Sin(radKiraKira);
            var endKiraKiraY = 15 - 15 * Math.Cos(radKiraKira);
            self.KiraKiraArcEnd = new Point(endKiraKiraX, endKiraKiraY);
            self.KiraKiraLargeArc = radKiraKira >= Math.PI;

            if (cond > 99) self.CondFontSize = 12;
            else self.CondFontSize = 14;
        }

        private static void ConditionTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (ConditionIndicator)d;
            switch((ConditionType)e.NewValue) {
                case ConditionType.Brilliant:
                    self.ConditionBrush = Brushes.Yellow;
                    self.NormBrush = Brushes.White;
                    break;
                case ConditionType.OrangeTired:
                    self.NormBrush = self.ConditionBrush = _brushOrangeTired;
                    break;
                case ConditionType.RedTired:
                    self.NormBrush = self.ConditionBrush = _brushRedTired;
                    break;
                case ConditionType.Tired:
                    self.NormBrush = self.ConditionBrush = _brushTired;
                    break;
                default:
                    self.NormBrush = self.ConditionBrush = Brushes.White;
                    break;
            }
        }

        public ConditionIndicator()
        {
            InitializeComponent();
        }
    }
}
