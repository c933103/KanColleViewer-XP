using LynLogger.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.Views
{
    public class ShipStatusModel : NotificationSourceObject
    {
        private static readonly IReadOnlyDictionary<Expression<Func<object, object>>, List<Expression<Func<object, object>>>> PropertyDependencies =
            new Dictionary<Expression<Func<object, object>>, List<Expression<Func<object, object>>>> {
                [o => ((ShipStatusModel)o).Ships] = new List<Expression<Func<object, object>>> {
                    o => ((ShipStatusModel)o).ShipSortMode},
                [o => ((ShipStatusModel)o).RemainingExp] = new List<Expression<Func<object, object>>> {
                    o => ((ShipStatusModel)o).SelectedShip,
                    o => ((ShipStatusModel)o).TargetLevel },
                [o => ((ShipStatusModel)o).RamaingCount] = new List<Expression<Func<object, object>>> {
                    o => ((ShipStatusModel)o).RemainingExp,
                    o => ((ShipStatusModel)o).IsFlagship,
                    o => ((ShipStatusModel)o).IsMvp,
                    o => ((ShipStatusModel)o).SelectedMapArea,
                    o => ((ShipStatusModel)o).TargetRank },
            };

        protected override IReadOnlyDictionary<Expression<Func<object, object>>, List<Expression<Func<object, object>>>> PropertyDependency { get { return PropertyDependencies; } }

        private Ship _selectedShip;
        private string _selectedMapArea = "1-1";
        private int _targetLevel;
        private bool _isMvp;
        private bool _isFlagship;
        private Rank _rank;
        private List<SortRequest<Ship>> sortMode = new List<SortRequest<Ship>>() {
            new SortRequest<Ship> { SortKey = _avaliableSorts[0] }
        };

        public IEnumerable<string> MapAreas { get { return Data.MapExperienceTable.Instance.Keys; } }
        public IEnumerable<Rank> Ranks { get { return _ranks; } }
        public IEnumerable<SortRequest<Ship>> ShipSortMode { get { return new LinkedList<SortRequest<Ship>>(sortMode); } }
        
        public IEnumerable<Ship> Ships
        {
            get
            {
                if(DataStore.Instance == null) return null;
                IOrderedEnumerable<Ship> o;
                if(sortMode[0].SortAscending) {
                    o = DataStore.Instance.Ships.Select(x => x.Value).OrderBy(sortMode[0].SortKey.SortKeySelector);
                } else {
                    o = DataStore.Instance.Ships.Select(x => x.Value).OrderByDescending(sortMode[0].SortKey.SortKeySelector);
                }
                foreach(var sm in sortMode.Skip(1)) {
                    if(sm.SortAscending) {
                        o = o.ThenBy(sm.SortKey.SortKeySelector);
                    } else {
                        o = o.ThenByDescending(sm.SortKey.SortKeySelector);
                    }
                }
                return new LinkedList<Ship>(o);
            }
        }

        public Ship SelectedShip
        {
            get { return _selectedShip; }
            set
            {
                if(_selectedShip == value) return;
                _selectedShip = value;
                RaisePropertyChanged();
            }
        }

        public string SelectedMapArea
        {
            get { return _selectedMapArea; }
            set
            {
                if(_selectedMapArea == value) return;
                _selectedMapArea = value;
                RaisePropertyChanged();
            }
        }

        public int TargetLevel
        {
            get { return _targetLevel; }
            set
            {
                if(_targetLevel == value) return;
                _targetLevel = value;
                RaisePropertyChanged();
            }
        }

        public bool IsMvp
        {
            get { return _isMvp; }
            set
            {
                if(value == _isMvp) return;
                _isMvp = value;
                RaisePropertyChanged();
            }
        }

        public bool IsFlagship
        {
            get { return _isFlagship; }
            set
            {
                if(value == _isFlagship) return;
                _isFlagship = value;
                RaisePropertyChanged();
            }
        }

        public Rank TargetRank
        {
            get { return _rank; }
            set
            {
                if(value == _rank) return;
                _rank = value;
                RaisePropertyChanged();
            }
        }

        public int RemainingExp
        {
            get
            {
                var currentLevel = SelectedShip?.Level ?? int.MaxValue;
                if(currentLevel >= TargetLevel) return 0;

                if(currentLevel <= 99 && TargetLevel >= 100) return int.MaxValue;
                if(currentLevel == 150) return 0;
                if(currentLevel == 99) return 0;
                
                return Data.LevelExperienceTable.Instance[TargetLevel] - Data.LevelExperienceTable.Instance[currentLevel+1] + SelectedShip.ExpNext;
            }
        }

        public int RamaingCount
        {
            get
            {
                var mapExp = Data.MapExperienceTable.Instance[SelectedMapArea];
                double multiplier;
                switch(TargetRank) {
                    case Rank.S:
                        multiplier = 1.2;
                        break;
                    case Rank.A:
                    case Rank.B:
                        multiplier = 1;
                        break;
                    case Rank.C:
                        multiplier = 0.8;
                        break;
                    case Rank.D:
                        multiplier = 0.7;
                        break;
                    case Rank.E:
                    default:
                        multiplier = 0.5;
                        break;
                }
                if(IsFlagship) multiplier *= 1.5;
                if((TargetRank != Rank.E) && IsMvp) multiplier *= 2;

                var expGet = (int)(mapExp * multiplier);
                return (int)Math.Ceiling(1.0 * RemainingExp / expGet);
            }
        }

        public ShipStatusModel()
        {
            DataStore.OnDataStoreSwitch += (_, __) => RaisePropertyChanged(() => Ships);
            DataStore.ShipDataChanged += (ds, x) => {
                if(x == SelectedShip?.Id) {
                    RaisePropertyChanged(() => SelectedShip);
                }
                RaisePropertyChanged(() => Ships);
            };
            sortMode[0].PropertyChanged += HandleSortChange;
        }

        private void HandleSortChange(object s, PropertyChangedEventArgs e)
        {
            var sender = (SortRequest<Ship>)s;
            if(sortMode.Last() != sender && sender.SortKey == _avaliableSorts[0]) {
                sortMode.Remove(sender);
                sender.PropertyChanged -= HandleSortChange;
            }
            if(sortMode.Last().SortKey != _avaliableSorts[0]) {
                var defSort = new SortRequest<Ship> { SortKey = _avaliableSorts[0] };
                defSort.PropertyChanged += HandleSortChange;
                sortMode.Add(defSort);
            }
            RaisePropertyChanged(() => ShipSortMode);
        }

        public enum Rank { S, A, B, C, D, E }
        private static readonly Rank[] _ranks = new Rank[] { Rank.S, Rank.A, Rank.B, Rank.C, Rank.D, Rank.E };
        private static readonly SortMode<Ship>[] _avaliableSorts = new SortMode<Ship>[] {
            new SortMode<Ship>("(默认)", x => x.Id, true),
            new SortMode<Ship>("等级", x => x.Level),
            new SortMode<Ship>("下一级经验", x => x.ExpNext, true),
            new SortMode<Ship>("总经验", x => x.Exp),
            new SortMode<Ship>("HP", x => x.Hp),
            new SortMode<Ship>("最大HP", x => x.HpMax),
            new SortMode<Ship>("对空", x => x.AntiAir),
            new SortMode<Ship>("对潜", x => x.AntiSub),
            new SortMode<Ship>("装甲", x => x.Defense),
            new SortMode<Ship>("回避", x => x.Maneuver),
            new SortMode<Ship>("火力", x => x.Power),
            new SortMode<Ship>("射程", x => x.Range),
            new SortMode<Ship>("索敌", x => x.Scout),
            new SortMode<Ship>("雷装", x => x.Torpedo),
            new SortMode<Ship>("运", x => x.Luck),
            new SortMode<Ship>("入渠用油", x => x.DockFuel),
            new SortMode<Ship>("入渠用钢", x => x.DockSteel),
            new SortMode<Ship>("入渠时间", x => x.DockTime),
        };

        public class SortMode<T>
        {
            public string DisplayName { get; }
            public Func<T, int> SortKeySelector { get; }
            public bool DefaultAscending { get; }

            public SortMode(string dn, Func<T, int> oks, bool asc = false) { DisplayName = dn; SortKeySelector = oks; DefaultAscending = asc; }
        }

        public class SortRequest<T> : NotificationSourceObject
        {
            private SortMode<T> key;
            private bool ascending;

            public SortMode<T> SortKey
            {
                get { return key; }
                set
                {
                    if(key == value) return;
                    key = value;
                    RaisePropertyChanged();
                    SortAscending = value.DefaultAscending;
                }
            }

            public bool SortAscending
            {
                get { return ascending; }
                set
                {
                    if(ascending == value) return;
                    ascending = value;
                    RaisePropertyChanged();
                }
            }
            public IEnumerable<SortMode<Ship>> SortModes { get { return _avaliableSorts; } }
        }
    }
}
