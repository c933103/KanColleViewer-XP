using LynLogger.DataStore.LogBook;
using LynLogger.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.Logger
{
    static class DrillLogger
    {
        private static Action _onNewLogEntry;
        public static event Action OnNewLogEntry
        {
            add { _onNewLogEntry += value.MakeWeak(x => _onNewLogEntry -= x); }
            remove { }
        }

        enum State { Homeport, Battle, NightBattle, BattleResult }
        private static State _state;
        static DrillInfo _info;
        static bool _initialized;

        public static void Init()
        {
            if (_initialized) return;
            _initialized = true;
            LynLoggerMain.OnInstanceCreate += i => {
                i.MapStartNextObserver.OnMapNext += map => {
                    _state = State.Homeport;
                };
                i.PracticeEnemyInfoObserver.OnPracticeEnemyInfo += info => {
                    _info = new DrillInfo() {
                        Briefing = info
                    };
                    _state = State.Battle;
                };
                i.BattleObserver.OnBattle += battle => {
                    if (_state == State.Battle) {
                        _info.Battle = battle;
                        _state = battle.HasNightWar ? State.NightBattle : State.BattleResult;
                    } else if (_state == State.NightBattle) {
                        _info.Battle = _info.Battle.Clone();
                        _info.Battle.NightWar = battle.NightWar;
                        _state = State.BattleResult;
                    }
                };
                i.BattleResultObserver.OnBattleResult += battleResult => {
                    if (_state == State.NightBattle || _state == State.BattleResult) {
                        _info.Result = battleResult;
                        DataStore.Store.Current.CurrentLogbook.DrillLog.Append(_info, 0);
                        DataStore.Store.Current.Weekbook.DrillLog.Append(_info, 0);
                        _info = null;
                        if (_onNewLogEntry != null)
                            _onNewLogEntry();
                    }
                    _state = State.Homeport;
                };
                i.PortObserver.OnPortAccess += () => {
                    _state = State.Homeport;
                    _info = null;
                };
            };
        }
    }
}
