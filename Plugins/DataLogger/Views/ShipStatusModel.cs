using LynLogger.DataStore;
using LynLogger.DataStore.MasterInfo;
using LynLogger.Models;
using LynLogger.Models.Battling;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace LynLogger.Views
{
    public partial class ShipStatusModel : NotificationSourceObject<ShipStatusModel>
    {
        protected override IReadOnlyDictionary<string, IReadOnlyCollection<string>> PropertyDependency =>
            new Dictionary<string, IReadOnlyCollection<string>> {
                [nameof(Ships)] = new string[] {
                    nameof(ShipSortMode),
                    nameof(CustomComparerSource)},
                [nameof(RemainingExp)] = new string[] {
                    nameof(CurrentLevel),
                    nameof(ExpNext),
                    nameof(TargetLevel) },
                [nameof(RemaingCount)] = new string[] {
                    nameof(RemainingExp),
                    nameof(IsFlagship),
                    nameof(IsMvp),
                    nameof(SelectedMapArea),
                    nameof(TargetRank) },
                [nameof(FuelReq)] = new string[] {
                    nameof(SelectedShip) },
                [nameof(AmmoReq)] = new string[] {
                    nameof(SelectedShip) },
                [nameof(TargetLevel)] = new string[] {
                    nameof(SelectedShip) },
                [nameof(CurrentLevel)] = new string[] {
                    nameof(SelectedShip) },
                [nameof(ExpNext)] = new string[] {
                    nameof(SelectedShip) }
            };

        private Ship _selectedShip;
        private string _selectedMapArea = "1-1";
        private int? _currentLevel;
        private int? _expNext;
        private int? _targetLevel;
        private int _autoLv = 1;
        private bool _isMvp;
        private bool _isFlagship;
        private Ranking _rank;
        private List<SortRequest<Ship>> sortMode;
        private List<ComparerBase<Ship>> avaliableSorts;

        public IEnumerable<string> MapAreas => Data.MapExperienceTable.Instance.Keys;
        public IEnumerable<Ranking> Ranks => _ranks;
        public IEnumerable<SortRequest<Ship>> ShipSortMode => new LinkedList<SortRequest<Ship>>(sortMode);
        
        public IEnumerable<Ship> Ships
        {
            get
            {
                while(true) {
                    if(Store.Current == null) return null;
                    IOrderedEnumerable<Ship> o;
                    if(sortMode[0].SortAscending) {
                        o = Store.Current.Ships.OrderBy(x => x, sortMode[0].SortKey);
                    } else {
                        o = Store.Current.Ships.OrderByDescending(x => x, sortMode[0].SortKey);
                    }
                    foreach(var sm in sortMode.Skip(1)) {
                        if(sm.SortAscending) {
                            o = o.ThenBy(x => x, sm.SortKey);
                        } else {
                            o = o.ThenByDescending(x => x, sm.SortKey);
                        }
                    }
                    try {
                        return new LinkedList<Ship>(o);
                    } catch (InvalidOperationException) { }
                }
            }
        }

        public Ship SelectedShip
        {
            get { return _selectedShip; }
            set
            {
                if(_selectedShip == value) return;
                _selectedShip = value;
                if (value != null) {
                    _autoLv = Grabacr07.KanColleWrapper.KanColleClient.Current?.Homeport?.Organization?.Ships[value.Id]?.Info?.NextRemodelingLevel ?? 1;
                    if (value.Level >= _autoLv) _autoLv = 99;
                    if (value.Level >= _autoLv) _autoLv = 150;
                }
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

        public int? CurrentLevel
        {
            get { return _currentLevel ?? _selectedShip?.Level ?? 1; }
            set
            {
                if (_currentLevel == value) return;
                _currentLevel = value;
                RaisePropertyChanged();
            }
        }

        public int? ExpNext
        {
            get { return _expNext ?? _selectedShip?.ExpNext ?? 0; }
            set
            {
                if (_expNext == value) return;
                _expNext = value;
                RaisePropertyChanged();
            }
        }

        public int? TargetLevel
        {
            get { return _targetLevel ?? _autoLv; }
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

        public Ranking TargetRank
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
                if(CurrentLevel >= TargetLevel) return 0;

                if(CurrentLevel <= 99 && TargetLevel >= 100) return int.MaxValue;
                if(CurrentLevel == 150) return 0;
                if(CurrentLevel == 99) return 0;

                return Data.LevelExperienceTable.Instance[TargetLevel.Value] - Data.LevelExperienceTable.Instance[CurrentLevel.Value + 1] + ExpNext.Value;
            }
        }

        public int RemaingCount
        {
            get
            {
                var mapExp = Data.MapExperienceTable.Instance[SelectedMapArea];
                double multiplier;
                switch(TargetRank) {
                    case Ranking.S:
                        multiplier = 1.2;
                        break;
                    case Ranking.A:
                    case Ranking.B:
                        multiplier = 1;
                        break;
                    case Ranking.C:
                        multiplier = 0.8;
                        break;
                    case Ranking.D:
                        multiplier = 0.7;
                        break;
                    case Ranking.E:
                    default:
                        multiplier = 0.5;
                        break;
                }
                if(IsFlagship) multiplier *= 1.5;
                if((TargetRank != Ranking.E) && IsMvp) multiplier *= 2;

                var expGet = (int)(mapExp * multiplier);
                return (int)Math.Ceiling(1.0 * RemainingExp / expGet);
            }
        }

        public int FuelReq => SelectedShip == null ? 0 : SelectedShip.Fuel - SelectedShip.MaxFuel;
        public int AmmoReq => SelectedShip == null ? 0 : SelectedShip.Ammo - SelectedShip.MaxAmmo;

        public ShipStatusModel()
        {
            Store.OnDataStoreSwitch += _ => RaiseMultiPropertyChanged(nameof(Ships));
            Store.OnDataStoreCreate += store => store.OnShipDataChange += (ds, x) => {
                if(x == SelectedShip?.Id) {
                    RaiseMultiPropertyChanged(nameof(SelectedShip));
                }
                RaiseMultiPropertyChanged(nameof(Ships));
            };
            sortMode = new List<SortRequest<Ship>>() {
                new SortRequest<Ship>(this) { SortKey = _builtinSorts[0] }
            };
            avaliableSorts = new List<ComparerBase<Ship>>(_builtinSorts);
            avaliableSorts.Add(new CustomComparerWrapper(this));
            sortMode[0].PropertyChanged += HandleSortChange;
        }

        private void HandleSortChange(object s, PropertyChangedEventArgs e)
        {
            var sender = (SortRequest<Ship>)s;
            if(sortMode.Last() != sender && sender.SortKey == _builtinSorts[0]) {
                sortMode.Remove(sender);
                sender.PropertyChanged -= HandleSortChange;
            }
            if(sortMode.Last().SortKey != _builtinSorts[0]) {
                var defSort = new SortRequest<Ship>(this) { SortKey = _builtinSorts[0] };
                defSort.PropertyChanged += HandleSortChange;
                sortMode.Add(defSort);
            }
            RaiseMultiPropertyChanged(nameof(ShipSortMode));
        }

        private static readonly Ranking[] _ranks = (Ranking[])Enum.GetValues(typeof(Ranking));
        private static readonly ComparerBase<Ship>[] _builtinSorts = new ComparerBase<Ship>[] {
            new WeightComparerBase<Ship>("(默认)", x => x.Id, true),
            new WeightComparerBase<Ship>("等级", x => x.Level),
            new WeightComparerBase<Ship>("下一级经验", x => x.ExpNext, true),
            new WeightComparerBase<Ship>("总经验", x => x.Exp),
            new ComparerBase<Ship>("舰种", (a, b) => a.ShipInfo.TypeName.CompareTo(b.ShipInfo.TypeName), true),
            new WeightComparerBase<Ship>("舰名", x => x.ShipInfo.ShipId, true),
            new WeightComparerBase<Ship>("当前HP", x => x.Hp),
            new WeightComparerBase<Ship>("最大HP", x => x.HpMax),
            new WeightComparerBase<Ship>("对空", x => x.AntiAir),
            new WeightComparerBase<Ship>("对潜", x => x.AntiSub),
            new WeightComparerBase<Ship>("装甲", x => x.Defense),
            new WeightComparerBase<Ship>("回避", x => x.Maneuver),
            new WeightComparerBase<Ship>("火力", x => x.Power),
            new WeightComparerBase<Ship>("射程", x => x.Range),
            new WeightComparerBase<Ship>("索敌", x => x.Scout),
            new WeightComparerBase<Ship>("雷装", x => x.Torpedo),
            new WeightComparerBase<Ship>("运", x => x.Luck),
            new WeightComparerBase<Ship>("入渠用油", x => x.DockFuel),
            new WeightComparerBase<Ship>("入渠用钢", x => x.DockSteel),
            new WeightComparerBase<Ship>("入渠时间", x => x.DockTime),
        };

        public class WeightComparerBase<T> : ComparerBase<T>
        {
            public Func<T, double> WeightFunction { get; }

            public WeightComparerBase(string dn, Func<T, double> weight, bool asc = false)
            :base(dn, null, asc) { WeightFunction = weight; }

            public override int Compare(T x, T y)
            {
                return WeightFunction(x).CompareTo(WeightFunction(y));
            }
        }

        public class ComparerBase<T> : IComparer<T>
        {
            public string DisplayName { get; }
            public Func<T, T, int> CompareFunction { get; }
            public bool DefaultAscending { get; }

            public ComparerBase(string dn, Func<T, T, int> cmp, bool asc = false) { DisplayName = dn; CompareFunction = cmp; DefaultAscending = asc; }

            public virtual int Compare(T x, T y)
            {
                return CompareFunction(x, y);
            }
        }

        public class SortRequest<T> : NotificationSourceObject<SortRequest<T>>
        {
            private ComparerBase<T> key;
            private bool ascending;
            private readonly ShipStatusModel parent;

            public ComparerBase<T> SortKey
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

            public SortRequest(ShipStatusModel parent) { this.parent = parent; }
            public IEnumerable<ComparerBase<Ship>> SortModes => parent.avaliableSorts;
        }
    }
}
