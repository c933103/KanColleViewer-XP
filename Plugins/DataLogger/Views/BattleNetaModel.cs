using LynLogger.Models.Battling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LynLogger.Views
{
    public class BattleNetaModel : Models.NotificationSourceObject
    {
        protected override IDictionary<Expression<Func<object, object>>, List<Expression<Func<object, object>>>> PropertyDependency
        {
            get
            {
                return new Dictionary<Expression<Func<object, object>>, List<Expression<Func<object, object>>>> {
                    [o => ((BattleNetaModel)o).BasicExp] = new List<Expression<Func<object, object>>> {
                        o => ((BattleNetaModel)o).Battle },
                    [o => ((BattleNetaModel)o).IsDrill] = new List<Expression<Func<object, object>>> {
                        o => ((BattleNetaModel)o).Battle },
                    [o => ((BattleNetaModel)o).SupplementMapNextInfo] = new List<Expression<Func<object, object>>> {
                        o => ((BattleNetaModel)o).MapNext },
                    [o => ((BattleNetaModel)o).ShowBattleResult] = new List<Expression<Func<object, object>>> {
                        o => ((BattleNetaModel)o).Battle,
                        o => ((BattleNetaModel)o).MapNext,
                        o => ((BattleNetaModel)o).PracticeEnemy },
                    [o => ((BattleNetaModel)o).ShowBattleProcess] = new List<Expression<Func<object, object>>> {
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

        private bool _showBattleProcess = false;
        public bool ShowBattleProcess
        {
            get { var t = _showBattleProcess; _showBattleProcess = false; return t; }
            set
            {
                if (_showBattleProcess == value) return;
                _showBattleProcess = value;
                RaisePropertyChanged();
            }
        }

        private BattleProcess _battle;
        public BattleProcess Battle
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

        public int BasicExp
        {
            get
            {
                if(Battle == null) return 2;
                switch(ShowInfo) {
                    case ViewShowInfo.MapNext:
                        {
                            var exp = Data.MapExperienceTable.Instance[string.Format("{0}-{1}", MapNext.MapLocation.MapAreaId, MapNext.MapLocation.MapSectId)];
                            if (exp != 2) return exp;
                            Models.BattleInfo inf;
                            if(DataStore.Store.Current.EnemyInfo.TryGetValue(MapNext.MapLocation, out inf)) {
                                return inf.BaseExp;
                            }
                            return 2;
                        }
                    case ViewShowInfo.PracticeEnemyInfo:
                        return Battle.DrillBasicExp;
                }
                return 2;
            }
        }

        public Models.BattleInfo? SupplementMapNextInfo
        {
            get
            {
                if (MapNext == null) return null;
                Models.BattleInfo inf;
                if (DataStore.Store.Current.EnemyInfo.TryGetValue(MapNext.MapLocation, out inf)) {
                    return inf;
                }
                return null;
            }
        }

        public bool IsDrill
        {
            get { return ShowInfo == ViewShowInfo.PracticeEnemyInfo; }
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
                            var b = Battle.Clone();
                            b.NightWar = a.NightWar;
                            Battle = b;
                            break;
                    }
                    ShowBattleProcess = true;
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
