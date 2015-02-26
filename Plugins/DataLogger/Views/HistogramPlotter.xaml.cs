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

namespace LynLogger.Views
{
    /// <summary>
    /// Interaction logic for HistogramPlotter.xaml
    /// </summary>
    public partial class HistogramPlotter : UserControl
    {
        public static readonly DependencyProperty dpPlotData = DependencyProperty.Register("PlotData", typeof(IEnumerable<KeyValuePair<long, double>>), typeof(HistogramPlotter), new PropertyMetadata(new PropertyChangedCallback(PlotChanged)));
        public static readonly DependencyProperty dpAverageDelta = DependencyProperty.Register("AverageDelta", typeof(bool?), typeof(HistogramPlotter), new PropertyMetadata(new PropertyChangedCallback(PlotChanged)));

        private Size plotArea;

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
            PlotDataHistory.Width = PlotDataIncrement.Width = PlotDataDecrement.Width = e.NewSize.Width;
            PlotDataHistory.Height = PlotDataIncrement.Height = PlotDataDecrement.Height = e.NewSize.Height;

            ReplotGrid();
            ReplotData();
        }

        private void ReplotData()
        {
            if(PlotData == null) return;

            var dat = new LinkedList<KeyValuePair<long,double>>(PlotData);
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

            long minTs = long.MaxValue;
            long maxTs = long.MinValue;
            double minVal = double.MaxValue;
            double maxVal = double.MinValue;
            double minDelta = 0;
            double maxDelta = 0;
            double lastDelta = 0;
            bool avgDelta = AverageDelta ?? false;
            LinkedListNode<KeyValuePair<long,double>> node = dat.First;

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

                lnHist.Points.Add(new Point((ts - minTs) / tsDiff * plotArea.Width, (maxVal - val) / valDiff * plotArea.Height));
                if(avgDelta) {
                    double intermidiateTs = ((double)ts + node.Previous.Value.Key) / 2;
                    lnPrev.Points.Add(new Point((intermidiateTs - minTs) / tsDiff * plotArea.Width, (maxDelta - lastDelta) / deltaDiff * plotArea.Height));
                    if(lnPrev != lnThis) {
                        lnPrev.Points.Add(new Point((intermidiateTs - minTs) / tsDiff * plotArea.Width, (maxDelta) / deltaDiff * plotArea.Height));
                        lnThis.Points.Add(new Point((intermidiateTs - minTs) / tsDiff * plotArea.Width, (maxDelta) / deltaDiff * plotArea.Height));
                    }
                    lnThis.Points.Add(new Point((intermidiateTs - minTs) / tsDiff * plotArea.Width, (maxDelta - delta) / deltaDiff * plotArea.Height));
                } else {
                    if(lnPrev != lnThis) {
                        long tsDelta = ts - node.Previous.Value.Key;
                        double intermidiateTs = ts - (val / delta * tsDelta);
                        lnPrev.Points.Add(new Point((intermidiateTs - minTs) / tsDiff * plotArea.Width, (maxDelta) / deltaDiff * plotArea.Height));
                        lnThis.Points.Add(new Point((intermidiateTs - minTs) / tsDiff * plotArea.Width, (maxDelta) / deltaDiff * plotArea.Height));
                    }
                    lnThis.Points.Add(new Point((ts - minTs) / tsDiff * plotArea.Width, (maxDelta - delta) / deltaDiff * plotArea.Height));
                }

                node = node.Next;
                lastDelta = delta;
            }
            if(avgDelta) {
                PolyLineSegment lnPrev = (lastDelta < 0) ? lnDecr : lnIncr;
                lnPrev.Points.Add(new Point((dat.Last.Value.Key - minTs) / tsDiff * plotArea.Width, (maxDelta - lastDelta) / deltaDiff * plotArea.Height));
            } else {

            }
            lnIncr.Points.Add(new Point((dat.Last.Value.Key - minTs) / tsDiff * plotArea.Width, (maxDelta) / deltaDiff * plotArea.Height));
            lnDecr.Points.Add(new Point((dat.Last.Value.Key - minTs) / tsDiff * plotArea.Width, (maxDelta) / deltaDiff * plotArea.Height));

            PathFigure figHist = new PathFigure() { IsClosed = false, IsFilled = false };
            PathFigure figIncr = new PathFigure() { IsClosed = true, IsFilled = true };
            PathFigure figDecr = new PathFigure() { IsClosed = true, IsFilled = true };
            figHist.StartPoint = new Point((dat.First.Value.Key - minTs) / tsDiff * plotArea.Width, (maxVal - dat.First.Value.Value) / valDiff * plotArea.Height);
            figDecr.StartPoint = figIncr.StartPoint = new Point((dat.First.Value.Key - minTs) / tsDiff * plotArea.Width, maxDelta / deltaDiff * plotArea.Height);

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
            d.Height += 30;
            d.Width += 35;
            d.Height = Math.Min(d.Height, constraint.Height);
            d.Width = Math.Min(d.Width, constraint.Width);
            MainGrid.Measure(d);
            return d;
        }
    }
}
