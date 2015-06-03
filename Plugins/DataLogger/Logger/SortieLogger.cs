using LynLogger.DataStore.LogBook;
using LynLogger.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.Logger
{
    static class SortieLogger
    {
        private static Action _onNewLogEntry;
        public static event Action OnNewLogEntry
        {
            add { _onNewLogEntry += value.MakeWeak(x => _onNewLogEntry -= x); }
            remove { }
        }

        enum State { Homeport, MapNext, Battle, NightBattle, BattleResult }
        private static State _state;
        static SortieInfo _info;
        static bool _initialized;

        public static void Init()
        {
            if (_initialized) return;
            _initialized = true;
            LynLoggerMain.OnInstanceCreate += i => {
                i.MapStartNextObserver.OnMapNext += map => {
                    if (_state == State.Homeport) {
                        _info = new SortieInfo() {
                            MapId = string.Format("{0}-{1}", map.MapAreaId, map.MapSectionId),
                            Nodes = new SortieInfo.Node[] {new SortieInfo.Node() {
                                Route = map
                            } }
                        };
                        _state = State.Battle;
                    } else if (_state == State.MapNext || _state == State.Battle) {
                        var nodes = _info.Nodes;
                        Array.Resize(ref nodes, _info.Nodes.Length + 1);
                        _info.Nodes = nodes;
                        _info.Nodes[nodes.Length - 1] = new SortieInfo.Node() {
                            Route = map
                        };
                        _state = State.Battle;
                    }
                };
                i.BattleObserver.OnBattle += battle => {
                    if (_state == State.Battle) {
                        _info.Nodes.Last().Battle = battle;
                        if (battle.HasNightWar) {
                            _state = State.NightBattle;
                        } else {
                            _state = State.BattleResult;
                        }
                    } else if (_state == State.NightBattle) {
                        _info.Nodes.Last().Battle = _info.Nodes.Last().Battle.Clone();
                        _info.Nodes.Last().Battle.NightWar = battle.NightWar;
                        _state = State.BattleResult;
                    }
                };
                i.BattleResultObserver.OnBattleResult += battleResult => {
                    if (_state == State.NightBattle || _state == State.BattleResult) {
                        _info.Nodes.Last().Result = battleResult;
                    }
                    _state = State.MapNext;
                };
                i.PortObserver.OnPortAccess += () => {
                    _state = State.Homeport;
                    if (_info != null) {
                        DataStore.Store.Current.CurrentLogbook.SortieLog.Append(_info, 0);
                        DataStore.Store.Current.Weekbook.SortieLog.Append(_info, 0);
                        _info = null;
                        if (_onNewLogEntry != null)
                            _onNewLogEntry();
                    }
                };
            };
        }
    }
}
