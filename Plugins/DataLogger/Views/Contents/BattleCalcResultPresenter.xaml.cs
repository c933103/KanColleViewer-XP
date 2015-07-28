using LynLogger.Models.Battling;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// BattleCalcResultPresenter.xaml 的交互逻辑
    /// </summary>
    public partial class BattleCalcResultPresenter : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty dpBattle = DependencyProperty.Register(nameof(Battle), typeof(BattleProcess), typeof(BattleCalcResultPresenter), new PropertyMetadata(
            (o, e) => ((BattleCalcResultPresenter)o).RaiseCommPropertyChanged(e.Property.Name)
        ));

        public static readonly DependencyProperty dpIsDrill = DependencyProperty.Register(nameof(IsDrill), typeof(bool), typeof(BattleCalcResultPresenter), new PropertyMetadata(
            (o, e) => ((BattleCalcResultPresenter)o).RaiseCommPropertyChanged(e.Property.Name)
        ));

        public static readonly DependencyProperty dpBasicExp = DependencyProperty.Register(nameof(BasicExp), typeof(int), typeof(BattleCalcResultPresenter), new PropertyMetadata(
            (o, e) => ((BattleCalcResultPresenter)o).RaiseCommPropertyChanged(e.Property.Name)
        ));

        public event PropertyChangedEventHandler PropertyChanged;

        public BattleProcess Battle
        {
            get { return (BattleProcess)GetValue(dpBattle); }
            set { SetValue(dpBattle, value); }
        }

        public bool IsDrill
        {
            get { return (bool)GetValue(dpIsDrill); }
            set { SetValue(dpIsDrill, value); }
        }

        public int BasicExp
        {
            get { return (int)GetValue(dpBasicExp); }
            set { SetValue(dpBasicExp, value); }
        }

        public IEnumerable<KeyValuePair<BattleProcess.ShipHpStatus, TriState>> OurEndMvpStatus
        {
            get
            {
                if (Battle == null) yield break;

                var EndShips = Battle.OurShipBattleEndHp.ToList();
                var mvpRange = EndShips.Aggregate(new FuzzyInt(), (range, ship) => FuzzyInt.UpperRange(range, ship.DeliveredDamage));
                var fuzzy = EndShips.Select(x => x.DeliveredDamage).Where(x => x.LowerBound != x.UpperBound);
                if (fuzzy.Count() != 0) {
                    mvpRange.LowerBound = Math.Max(mvpRange.LowerBound, fuzzy.Max(x => x.UpperBound) / fuzzy.Count());
                }

                var inRangeState = EndShips.Count(x => (x.DeliveredDamage >= mvpRange) != TriState.No) == 1 ? TriState.Yes : TriState.DK;
                if (mvpRange.UpperBound == mvpRange.LowerBound) inRangeState = TriState.Yes;
                foreach (var ship in EndShips) {
                    yield return new KeyValuePair<BattleProcess.ShipHpStatus, TriState>(ship, (ship.DeliveredDamage >= mvpRange) != TriState.No ? inRangeState : TriState.No);
                    if (mvpRange.UpperBound == mvpRange.LowerBound && ship.DeliveredDamage.LowerBound == mvpRange.LowerBound) inRangeState = TriState.No;
                }
            }
        }

        public int EstimatedExp
        {
            get
            {
                if (Battle == null) return 2;
                int basic;
                if(!IsDrill) {
                    basic = BasicExp;
                    switch (Battle.Rank) {
                        case Ranking.S:
                            return (int)(basic * 1.2);
                        case Ranking.A:
                        case Ranking.B:
                            return basic;
                        case Ranking.C:
                            return (int)(basic * 0.8);
                        case Ranking.D:
                            return (int)(basic * 0.7);
                        case Ranking.E:
                            return (int)(basic * 0.5);
                    }
                } else {
                    basic = Battle.DrillBasicExp;
                    switch (Battle.Rank) {
                        case Ranking.S:
                            return (int)(basic * 1.2);
                        case Ranking.A:
                        case Ranking.B:
                            return basic;
                        case Ranking.C:
                            return (int)(basic * 0.64);
                        case Ranking.D:
                            return (int)(basic * 0.56);
                        case Ranking.E:
                            return (int)(basic * 0.4);
                    }
                }
                return 2;
            }
        }

        public BattleCalcResultPresenter()
        {
            InitializeComponent();
        }

        private void RaiseCommPropertyChanged(string name)
        {
            var handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(name));
                handler(this, new PropertyChangedEventArgs(nameof(OurEndMvpStatus)));
                handler(this, new PropertyChangedEventArgs(nameof(EstimatedExp)));
            }
        }
    }
}
