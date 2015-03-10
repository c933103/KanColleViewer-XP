using System.Collections.Generic;

namespace LynLogger.Views
{
    public class BattleNetaModel : Models.NotificationSourceObject
    {
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
                RaisePropertyChanged(o => Battle, o => BombardRound1, o => BombardRound2);
            }
        }

        public IReadOnlyList<Models.Battling.BattleStatus.BombardInfo> BombardRound1
        {
            get { return (Battle?.Bombards?.Count > 0) ? Battle.Bombards[0] : null; }
        }

        public IReadOnlyList<Models.Battling.BattleStatus.BombardInfo> BombardRound2
        {
            get { return (Battle?.Bombards?.Count > 1) ? Battle.Bombards[1] : null; }
        }

        public BattleNetaModel()
        {
            LynLoggerMain.OnInstanceCreate += i => {
                i.MapStartNextObserver.OnMapNext += a => MapNext = a;
                i.BattleObserver.OnBattle += a => Battle = a;
            };
        }
    }
}
