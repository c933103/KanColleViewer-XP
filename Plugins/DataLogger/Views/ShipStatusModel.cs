using LynLogger.Models;
using System;
using System.Collections.Generic;
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

        public IEnumerable<string> MapAreas { get { return Data.MapExperienceTable.Instance.Keys; } }
        public IEnumerable<Rank> Ranks { get { return _ranks; } }

        public IEnumerable<Ship> Ships
        {
            get
            {
                if(DataStore.Instance == null) return null;
                return new LinkedList<Ship>(DataStore.Instance.Ships.Select(x => x.Value));
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
        }

        public enum Rank { S, A, B, C, D, E }
        private static readonly Rank[] _ranks = new Rank[] { Rank.S, Rank.A, Rank.B, Rank.C, Rank.D, Rank.E };
    }
}
