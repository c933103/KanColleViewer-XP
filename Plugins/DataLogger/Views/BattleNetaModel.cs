using LynLogger.Models.Battling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LynLogger.Views
{
    public class BattleNetaModel : Models.NotificationSourceObject
    {
        protected override IReadOnlyDictionary<Expression<Func<object, object>>, List<Expression<Func<object, object>>>> PropertyDependency
        {
            get
            {
                return new Dictionary<Expression<Func<object, object>>, List<Expression<Func<object, object>>>> {
                    [o => ((BattleNetaModel)o).EstimatedExp] = new List<Expression<Func<object, object>>> {
                        o => ((BattleNetaModel)o).Battle },
                    [o => ((BattleNetaModel)o).OurEndMvpStatus] = new List<Expression<Func<object, object>>> {
                        o => ((BattleNetaModel)o).Battle },
                    [o => ((BattleNetaModel)o).ShowBattleResult] = new List<Expression<Func<object, object>>> {
                        o => ((BattleNetaModel)o).Battle,
                        o => ((BattleNetaModel)o).MapNext,
                        o => ((BattleNetaModel)o).PracticeEnemy },
                    [o => ((BattleNetaModel)o).ShowBattleStatus] = new List<Expression<Func<object, object>>> {
                        o => ((BattleNetaModel)o).MapNext,
                        o => ((BattleNetaModel)o).PracticeEnemy }
                };
            }
        }

        private ViewState _state = ViewState.AnticipateBattle;

        private MapNext _mapNext;
        public MapNext MapNext
        {
            get { return _mapNext; }
            set
            {
                if(value == _mapNext) return;
                _mapNext = value;
                RaisePropertyChanged();
            }
        }

        private BattleResult _battleResult;
        public BattleResult BattleResult
        {
            get { return _battleResult; }
            set
            {
                if (value == _battleResult) return;
                _battleResult = value;
                RaisePropertyChanged();
            }
        }

        private bool _showBattleResult = false;
        public bool ShowBattleResult
        {
            get { var t = _showBattleResult; _showBattleResult = false; return t; }
            set
            {
                if (_showBattleResult == value) return;
                _showBattleResult = value;
                RaisePropertyChanged();
            }
        }

        private bool _showBattleStatus = false;
        public bool ShowBattleStatus
        {
            get { var t = _showBattleStatus; _showBattleStatus = false; return t; }
            set
            {
                if (_showBattleStatus == value) return;
                _showBattleStatus = value;
                RaisePropertyChanged();
            }
        }

        private BattleStatus _battle;
        public BattleStatus Battle
        {
            get { return _battle; }
            set
            {
                if(value == _battle) return;
                _battle = value;
                RaisePropertyChanged();
            }
        }

        private PracticeEnemyInfo _practiceEnemy;
        public PracticeEnemyInfo PracticeEnemy
        {
            get { return _practiceEnemy; }
            set
            {
                if(value == _practiceEnemy) return;
                _practiceEnemy = value;
                RaisePropertyChanged();
            }
        }

        private ViewShowInfo _showInfo;
        public ViewShowInfo ShowInfo
        {
            get { return _showInfo; }
            set
            {
                if(value == _showInfo) return;
                _showInfo = value;
                RaisePropertyChanged();
            }
        }

        private FuzzyDouble _mvpRange;
        public FuzzyDouble MvpRange
        {
            get { return _mvpRange; }
            private set
            {
                _mvpRange = value;
                RaisePropertyChanged();
            }
        }

        public IEnumerable<KeyValuePair<BattleStatus.ShipHpStatus, TriState>> OurEndMvpStatus
        {
            get
            {
                if(Battle == null) yield break;

                var EndShips = Battle.OurShipBattleEndHp.ToList();
                MvpRange = EndShips.Aggregate(new FuzzyDouble(), (range, ship) => FuzzyDouble.UpperRange(range, ship.DeliveredDamage));
                var fuzzy = EndShips.Select(x => x.DeliveredDamage).Where(x => x.LowerBound != x.UpperBound);
                if(fuzzy.Count() != 0) {
                    MvpRange.LowerBound = Math.Max(MvpRange.LowerBound, fuzzy.Max(x => x.UpperBound) / fuzzy.Count());
                }

                var inRangeState = EndShips.Count(x => (x.DeliveredDamage >= MvpRange) != TriState.No) == 1 ? TriState.Yes : TriState.DK;
                foreach(var ship in EndShips) {
                    yield return new KeyValuePair<BattleStatus.ShipHpStatus, TriState>(ship, (ship.DeliveredDamage >= MvpRange) != TriState.No ? inRangeState : TriState.No);
                }
            }
        }

        public int EstimatedExp
        {
            get
            {
                if(Battle == null) return 2;
                int basic;
                switch(ShowInfo) {
                    case ViewShowInfo.MapNext:
                        basic = Data.MapExperienceTable.Instance[string.Format("{0}-{1}", MapNext.MapAreaId, MapNext.MapSectionId)];
                        switch(Battle.Rank) {
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
                        break;
                    case ViewShowInfo.PracticeEnemyInfo:
                        basic = Battle.DrillBasicExp;
                        switch(Battle.Rank) {
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
                        break;
                }
                return 2;
            }
        }

        public BattleNetaModel()
        {
            LynLoggerMain.OnInstanceCreate += i => {
                i.MapStartNextObserver.OnMapNext += a => {
                    MapNext = a;
                    _state = ViewState.AnticipateBattle;
                    ShowInfo = ViewShowInfo.MapNext;
                };
                i.PracticeEnemyInfoObserver.OnPracticeEnemyInfo += a => {
                    PracticeEnemy = a;
                    _state = ViewState.AnticipateBattle;
                    ShowInfo = ViewShowInfo.PracticeEnemyInfo;
                };
                i.BattleObserver.OnBattle += a => {
                    switch(_state) {
                        case ViewState.AnticipateBattle:
                            Battle = a;
                            if(a.HasNightWar) {
                                _state = ViewState.AnticipateNightBattle;
                            } else {
                                _state = ViewState.AnticipateBattle;
                            }
                            break;
                        case ViewState.AnticipateNightBattle:
                            if(a.NightWar == null) goto case ViewState.AnticipateBattle;
                            _state = ViewState.AnticipateBattle;
                            Battle.NightWar = a.NightWar;
                            RaiseMultiPropertyChanged(() => Battle);
                            break;
                    }
                    ShowBattleStatus = true;
                };
                i.BattleResultObserver.OnBattleResult += a => {
                    BattleResult = a;
                    ShowBattleResult = true;
                };
            };
        }

        enum ViewState { AnticipateBattle, AnticipateNightBattle }
        public enum ViewShowInfo { None, PracticeEnemyInfo, MapNext }
    }
}
