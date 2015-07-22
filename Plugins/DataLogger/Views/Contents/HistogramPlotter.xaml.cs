using LynLogger.Utilities;
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

namespace LynLogger.Views.Contents
{
    /// <summary>
    /// Interaction logic for HistogramPlotter.xaml
    /// </summary>
    public partial class HistogramPlotter : UserControl
    {
        public static readonly DependencyProperty dpPlotData = DependencyProperty.Register(nameof(PlotData), typeof(IEnumerable<KeyValuePair<long, double>>), typeof(HistogramPlotter), new PropertyMetadata(new PropertyChangedCallback(PlotChanged)));
        public static readonly DependencyProperty dpAverageDelta = DependencyProperty.Register(nameof(AverageDelta), typeof(bool?), typeof(HistogramPlotter), new PropertyMetadata(new PropertyChangedCallback(PlotChanged)));

        private Size plotArea;
        private long minTs = long.MaxValue;
        private long maxTs = long.MinValue;
        private double minVal = double.MaxValue;
        private double maxVal = double.MinValue;
        private LinkedList<KeyValuePair<long, double>> dat;

        public IEnumerable<KeyValuePair<long, double>> PlotData
        {
            get { return (IEnumerable<KeyValuePair<long, double>>)GetValue(dpPlotData); }
            set { SetValue(dpPlotData, value); }
        }

        public bool? AverageDelta
        {
            get { return (bool?)GetValue(dpAverageDelta); }
            set { SetValue(dpAverageDelta, value); }
        }

        public HistogramPlotter()
        {
            InitializeComponent();
            plotArea = PlotGrid.RenderSize;
            MainGrid.DataContext = this;

            ReplotGrid();
            ReplotData();
        }

        private static void PlotChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
            ((HistogramPlotter)o).ReplotData();
        }

        private void Plot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            plotArea = e.NewSize;

            PlotDataHistory.Width = PlotDataIncrement.Width = PlotDataDecrement.Width = plotArea.Width;
            PlotDataHistory.Height = PlotDataIncrement.Height = PlotDataDecrement.Height = plotArea.Height;
            DataCursor.Height = plotArea.Height;

            ReplotGrid();
            ReplotData();
        }

        private void ReplotData()
        {
            try {
                if(PlotData == null) return;

                dat = new LinkedList<KeyValuePair<long, double>>(PlotData);
                if(dat.First == null) return;
                if(dat.First.Next == null) {
                    PlotDataHistory.Data = new EllipseGeometry(new Point(plotArea.Width/2, plotArea.Height/2), 5, 5);
                    PlotDataDecrement.Data = new StreamGeometry();
                    PlotDataIncrement.Data = new StreamGeometry();
                    Delta0.Text = Delta1.Text = Delta2.Text = Delta3.Text = Delta4.Text = "0";
                    Val0.Text = Val1.Text = Val2.Text = Val3.Text = Val4.Text = dat.First.Value.Value.ToString("F2");
                    Time0.Text = Time1.Text = Time2.Text = Time3.Text = Time4.Text = Helpers.FromUnixTimestamp(dat.First.Value.Key).LocalDateTime.ToString();
                    return;
                }

                minTs = long.MaxValue;
                maxTs = long.MinValue;
                minVal = double.MaxValue;
                maxVal = double.MinValue;
                double minDelta = 0;
                double maxDelta = 0;
                double lastDelta = 0;
                double width = plotArea.Width;
                double height = plotArea.Height;
                bool avgDelta = AverageDelta ?? false;
                LinkedListNode<KeyValuePair<long, double>> node = dat.First;

                while(node != null) {
                    long ts = node.Value.Key;
                    double val = node.Value.Value;
                    double delta;
                    if(node.Previous != null) {
                        delta = val - node.Previous.Value.Value;
                        if(avgDelta) {
                            long tsd = (ts - node.Previous.Value.Key);
                            if(tsd < 3600) tsd = 3600;
                            delta = delta / (tsd / 3600.0);
                        }
                    } else {
                        delta = 0;
                    }

                    if(ts < minTs) minTs = ts;
                    if(ts > maxTs) maxTs = ts;
                    if(val < minVal) minVal = val;
                    if(val > maxVal) maxVal = val;
                    if(delta < minDelta) minDelta = delta;
                    if(delta > maxDelta) maxDelta = delta;

                    node = node.Next;
                }
                minTs -= 5;
                maxTs += 5;
                minVal -= 0.025;
                maxVal += 0.025;
                minDelta -= 0.025;
                maxDelta += 0.025;

                //TODO: Prettify the graph by ensuring a 0 appears on the vertical axis if the series has both positive and negative values.

                double tsDiff = maxTs - minTs;
                double valDiff = maxVal - minVal;
                double deltaDiff = maxDelta - minDelta;

                PolyLineSegment lnHist = new PolyLineSegment() { IsStroked = true };
                PolyLineSegment lnIncr = new PolyLineSegment();
                PolyLineSegment lnDecr = new PolyLineSegment();
                node = dat.First.Next;

                while(node != null) {
                    long ts = node.Value.Key;
                    double val = node.Value.Value;
                    double delta = val - node.Previous.Value.Value;
                    if(avgDelta) {
                        long tsd = (ts - node.Previous.Value.Key);
                        if(tsd < 3600) tsd = 3600;
                        delta = delta / (tsd / 3600.0);
                    }
                    PolyLineSegment lnPrev, lnThis;
                    lnPrev = (lastDelta < 0) ? lnDecr : lnIncr;
                    lnThis = (delta < 0) ? lnDecr : lnIncr;

                    lnHist.Points.Add(new Point((ts - minTs) / tsDiff * width, (maxVal - val) / valDiff * height));
                    if(avgDelta) {
                        //double intermidiateTs = ((double)ts + node.Previous.Value.Key) / 2;
                        lnPrev.Points.Add(new Point((node.Previous.Value.Key - minTs) / tsDiff * width, (maxDelta - lastDelta) / deltaDiff * height));
                        if(lnPrev != lnThis) {
                            lnPrev.Points.Add(new Point((node.Previous.Value.Key - minTs) / tsDiff * width, (maxDelta) / deltaDiff * height));
                            lnThis.Points.Add(new Point((node.Previous.Value.Key - minTs) / tsDiff * width, (maxDelta) / deltaDiff * height));
                        }
                        lnThis.Points.Add(new Point((node.Previous.Value.Key - minTs) / tsDiff * width, (maxDelta - delta) / deltaDiff * height));
                    } else {
                        if(lnPrev != lnThis) {
                            long tsDelta = ts - node.Previous.Value.Key;
                            double intermidiateTs = ts - (val / delta * tsDelta);
                            lnPrev.Points.Add(new Point((intermidiateTs - minTs) / tsDiff * width, (maxDelta) / deltaDiff * height));
                            lnThis.Points.Add(new Point((intermidiateTs - minTs) / tsDiff * width, (maxDelta) / deltaDiff * height));
                        }
                        lnThis.Points.Add(new Point((ts - minTs) / tsDiff * width, (maxDelta - delta) / deltaDiff * height));
                    }

                    node = node.Next;
                    lastDelta = delta;
                }
                if(avgDelta) {
                    PolyLineSegment lnPrev = (lastDelta < 0) ? lnDecr : lnIncr;
                    lnPrev.Points.Add(new Point((dat.Last.Value.Key - minTs) / tsDiff * width, (maxDelta - lastDelta) / deltaDiff * height));
                } else {

                }
                lnIncr.Points.Add(new Point((dat.Last.Value.Key - minTs) / tsDiff * width, (maxDelta) / deltaDiff * height));
                lnDecr.Points.Add(new Point((dat.Last.Value.Key - minTs) / tsDiff * width, (maxDelta) / deltaDiff * height));

                PathFigure figHist = new PathFigure() { IsClosed = false, IsFilled = false };
                PathFigure figIncr = new PathFigure() { IsClosed = true, IsFilled = true };
                PathFigure figDecr = new PathFigure() { IsClosed = true, IsFilled = true };
                figHist.StartPoint = new Point((dat.First.Value.Key - minTs) / tsDiff * width, (maxVal - dat.First.Value.Value) / valDiff * height);
                figDecr.StartPoint = figIncr.StartPoint = new Point((dat.First.Value.Key - minTs) / tsDiff * width, maxDelta / deltaDiff * height);

                figHist.Segments.Add(lnHist);
                figDecr.Segments.Add(lnDecr);
                figIncr.Segments.Add(lnIncr);

                PathGeometry geoHist = new PathGeometry();
                PathGeometry geoIncr = new PathGeometry();
                PathGeometry geoDecr = new PathGeometry();

                geoHist.Figures.Add(figHist);
                geoIncr.Figures.Add(figIncr);
                geoDecr.Figures.Add(figDecr);

                PlotDataHistory.Data = geoHist;
                PlotDataDecrement.Data = geoDecr;
                PlotDataIncrement.Data = geoIncr;

                TextBlock[] tbTime = new TextBlock[] { Time0, Time1, Time2, Time3, Time4 };
                TextBlock[] tbVal = new TextBlock[] { Val0, Val1, Val2, Val3, Val4 };
                TextBlock[] tbDelta = new TextBlock[] { Delta0, Delta1, Delta2, Delta3, Delta4 };

                for(int i = 0; i < 5; i++) {
                    tbTime[i].Text = Helpers.FromUnixTimestamp((long)(minTs + tsDiff / 10 * (1+2*i))).LocalDateTime.ToString();
                    tbDelta[i].Text = (minDelta + deltaDiff / 10 * (1+2*i)).ToString("F", 8);
                    tbVal[i].Text = (minVal + valDiff / 10 * (1+2*i)).ToString("F", 8);
                }
            } catch(InvalidOperationException) { }
        }

        private void ReplotGrid()
        {
            PathGeometry geo = new PathGeometry();
            PathFigure p;
            double xdiv = plotArea.Width / 10;
            double ydiv = plotArea.Height / 10;

            for(int i = 0; i < 5; i++) {
                p = new PathFigure();
                p.StartPoint = new Point(0, ydiv*(1+i*2));
                p.Segments.Add(new LineSegment(new Point(plotArea.Width, ydiv*(1+i*2)), true));
                geo.Figures.Add(p);
                p = new PathFigure();
                p.StartPoint = new Point(xdiv*(1+i*2), 0);
                p.Segments.Add(new LineSegment(new Point(xdiv*(1+i*2), plotArea.Height), true));
                geo.Figures.Add(p);
            }

            PlotGrid.Data = geo;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            TextBlock[] tbTime = new TextBlock[] { Time0, Time1, Time2, Time3, Time4 };
            TextBlock[] tbVal = new TextBlock[] { Val0, Val1, Val2, Val3, Val4 };
            TextBlock[] tbDelta = new TextBlock[] { Delta0, Delta1, Delta2, Delta3, Delta4 };
            Size c = new Size(constraint.Width / 5, constraint.Height / 5);
            Size d = new Size();

            for(int i = 0; i < 5; i++) {
                tbTime[i].Measure(c);
                tbDelta[i].Measure(c);
                tbVal[i].Measure(c);

                d.Height += Math.Max(tbDelta[i].DesiredSize.Height, tbVal[i].DesiredSize.Height);
                d.Width += tbTime[i].DesiredSize.Width;
            }
            d.Height += tbTime.Select(x => x.DesiredSize.Height).Max();
            d.Width += tbDelta.Select(x => x.DesiredSize.Width).Max();
            d.Width += tbVal.Select(x => x.DesiredSize.Width).Max();
            d.Height += 50;
            d.Width += 50;
            d.Height = Math.Min(d.Height, constraint.Height);
            d.Width = Math.Min(d.Width, constraint.Width);

            return base.MeasureOverride(d);
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (dat == null) return;

            var pt = e.GetPosition(PlotGrid);
            var tsVal = pt.X / plotArea.Width * (maxTs - minTs) + minTs;
            var minDiff = double.PositiveInfinity;
            var node = dat.First;
            while(node != null) {
                var diff = node.Value.Key - tsVal;
                if(diff >= 0) {
                    if (diff < minDiff) break;
                    node = node.Previous;
                    break;
                }
                minDiff = Math.Min(minDiff, -diff);
                if (node.Next == null) break;
                node = node.Next;
            }
            Canvas.SetLeft(DataCursor, (node.Value.Key - minTs) * plotArea.Width / (maxTs - minTs)-1);
            DataCursor.Visibility = Visibility.Visible;

            DataCursorValue.Text = string.Format("{0}\n{1} ({2:+#;-#;0})", Helpers.FromUnixTimestamp(node.Value.Key).LocalDateTime, node.Value.Value, (node.Value.Value - node.Previous?.Value.Value) ?? 0);
            if (node.Value.Key > (minTs / 2 + maxTs / 2)) {
                Canvas.SetLeft(DataCursorValue, double.NaN);
                Canvas.SetRight(DataCursorValue, (maxTs - node.Value.Key) * plotArea.Width / (maxTs - minTs));
            } else {
                Canvas.SetLeft(DataCursorValue, (node.Value.Key - minTs) * plotArea.Width / (maxTs - minTs));
                Canvas.SetRight(DataCursorValue, double.NaN);
            }
            if (node.Value.Value > (minVal / 2 + maxVal / 2)) {
                Canvas.SetBottom(DataCursorValue, double.NaN);
                Canvas.SetTop(DataCursorValue, (maxVal - node.Value.Value) * plotArea.Height / (maxVal - minVal));
            } else {
                Canvas.SetBottom(DataCursorValue, (node.Value.Value - minVal) * plotArea.Height / (maxVal - minVal));
                Canvas.SetTop(DataCursorValue, double.NaN);
            }
            DataCursorValue.Visibility = Visibility.Visible;
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            DataCursor.Visibility = Visibility.Collapsed;
            DataCursorValue.Visibility = Visibility.Collapsed;
        }
    }
}
