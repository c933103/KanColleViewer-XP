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
    public partial class ShipStatusModel : NotificationSourceObject
    {
        private static readonly IReadOnlyDictionary<Expression<Func<object, object>>, List<Expression<Func<object, object>>>> PropertyDependencies =
            new Dictionary<Expression<Func<object, object>>, List<Expression<Func<object, object>>>> {
                [o => ((ShipStatusModel)o).Ships] = new List<Expression<Func<object, object>>> {
                    o => ((ShipStatusModel)o).ShipSortMode,
                    o => ((ShipStatusModel)o).CustomComparerSource},
                [o => ((ShipStatusModel)o).RemainingExp] = new List<Expression<Func<object, object>>> {
                    o => ((ShipStatusModel)o).SelectedShip,
                    o => ((ShipStatusModel)o).TargetLevel },
                [o => ((ShipStatusModel)o).RamaingCount] = new List<Expression<Func<object, object>>> {
                    o => ((ShipStatusModel)o).RemainingExp,
                    o => ((ShipStatusModel)o).IsFlagship,
                    o => ((ShipStatusModel)o).IsMvp,
                    o => ((ShipStatusModel)o).SelectedMapArea,
                    o => ((ShipStatusModel)o).TargetRank },
                [o => ((ShipStatusModel)o).FuelReq] = new List<Expression<Func<object, object>>> {
                    o => ((ShipStatusModel)o).SelectedShip },
                [o => ((ShipStatusModel)o).AmmoReq] = new List<Expression<Func<object, object>>> {
                    o => ((ShipStatusModel)o).SelectedShip }
            };

        protected override IReadOnlyDictionary<Expression<Func<object, object>>, List<Expression<Func<object, object>>>> PropertyDependency { get { return PropertyDependencies; } }

        private Ship _selectedShip;
        private string _selectedMapArea = "1-1";
        private int _targetLevel;
        private bool _isMvp;
        private bool _isFlagship;
        private Ranking _rank;
        private List<SortRequest<Ship>> sortMode;
        private List<ComparerBase<Ship>> avaliableSorts;

        public IEnumerable<string> MapAreas { get { return Data.MapExperienceTable.Instance.Keys; } }
        public IEnumerable<Ranking> Ranks { get { return _ranks; } }
        public IEnumerable<SortRequest<Ship>> ShipSortMode { get { return new LinkedList<SortRequest<Ship>>(sortMode); } }
        
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

        public int FuelReq { get { return SelectedShip == null ? 0 : SelectedShip.Fuel - SelectedShip.MaxFuel; } }
        public int AmmoReq { get { return SelectedShip == null ? 0 : SelectedShip.Ammo - SelectedShip.MaxAmmo; } }

        public ShipStatusModel()
        {
            Store.OnDataStoreSwitch += (_, __) => RaiseMultiPropertyChanged(() => Ships);
            Store.OnDataStoreCreate += (_, store) => store.OnShipDataChange += (ds, x) => {
                if(x == SelectedShip?.Id) {
                    RaiseMultiPropertyChanged(() => SelectedShip);
                }
                RaiseMultiPropertyChanged(() => Ships);
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
            RaiseMultiPropertyChanged(() => ShipSortMode);
        }

        private static readonly Ranking[] _ranks = Enum.GetValues(typeof(Ranking)).Cast<Ranking>().ToArray();
        private static readonly ComparerBase<Ship>[] _builtinSorts = new ComparerBase<Ship>[] {
            new WeightComparerBase<Ship>("(默认)", x => x.Id, true),
            new WeightComparerBase<Ship>("等级", x => x.Level),
            new WeightComparerBase<Ship>("下一级经验", x => x.ExpNext, true),
            new WeightComparerBase<Ship>("总经验", x => x.Exp),
            new WeightComparerBase<Ship>("舰种", x => Helpers.LookupTypeId(x.ShipInfo.ShipId), true),
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

        public class SortRequest<T> : NotificationSourceObject
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
            public IEnumerable<ComparerBase<Ship>> SortModes { get { return parent.avaliableSorts; } }
        }
    }
}
