using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.Views
{
    public class BattleNetaModel : Models.NotificationSourceObject
    {
        private static readonly IReadOnlyDictionary<Expression<Func<object, object>>, List<Expression<Func<object, object>>>> PropertyDependencies =
            new Dictionary<Expression<Func<object, object>>, List<Expression<Func<object, object>>>> {
                [o => ((BattleNetaModel)o).BombardRound1] = new List<Expression<Func<object, object>>> {
                    o => ((BattleNetaModel)o).Battle},
                [o => ((BattleNetaModel)o).BombardRound2] = new List<Expression<Func<object, object>>> {
                    o => ((BattleNetaModel)o).Battle },
            };

        protected override IReadOnlyDictionary<Expression<Func<object, object>>, List<Expression<Func<object, object>>>> PropertyDependency { get { return PropertyDependencies; } }

        private ViewState _state = ViewState.AnticipateBattle;

        private Models.Battling.MapNext _mapNext;
        public Models.Battling.MapNext MapNext
        {
            get { return _mapNext; }
            set
            {
                if(value == _mapNext) return;
                _mapNext = value;
                RaisePropertyChanged();
            }
        }

        private Models.Battling.BattleStatus _battle;
        public Models.Battling.BattleStatus Battle
        {
            get { return _battle; }
            set
            {
                if(value == _battle) return;
                _battle = value;
                RaisePropertyChanged();
            }
        }

        public IReadOnlyList<Models.Battling.BattleStatus.BombardInfo> BombardRound1
        {
            get { return (Battle?.Bombards.Count > 0) ? Battle.Bombards[0] : null; }
        }

        public IReadOnlyList<Models.Battling.BattleStatus.BombardInfo> BombardRound2
        {
            get { return (Battle?.Bombards.Count > 1) ? Battle.Bombards[1] : null; }
        }

        public BattleNetaModel()
        {
            LynLoggerMain.OnInstanceCreate += i => {
                i.MapStartNextObserver.OnMapNext += a => {
                    MapNext = a;
                    _state = ViewState.AnticipateBattle;
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
                            if(!a.HasNightWar) goto case ViewState.AnticipateBattle;
                            _state = ViewState.AnticipateBattle;
                            Battle.NightWar = a.NightWar;
                            RaiseMultiPropertyChanged(() => Battle);
                            break;
                    }
                };
            };
        }

        enum ViewState { AnticipateBattle, AnticipateNightBattle }
    }
}
