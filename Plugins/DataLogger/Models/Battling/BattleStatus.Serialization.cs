using LynLogger.DataStore.Extensions;
using LynLogger.DataStore.MasterInfo;
using LynLogger.DataStore.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.Models.Battling
{
    public partial class BattleStatus : AbstractDSSerializable<BattleStatus>, ICloneable, IShipInfoHolder
    {
        protected override IDictionary<ulong, HandlerInfo> CustomFieldHandlers
        {
            get
            {
                return new Dictionary<ulong, HandlerInfo>() {
                    [0] = new HandlerInfo(
                        x => x.ZwEnemyShips.GetSerializationInfo(k => k.GetSerializationInfo()),
                        (o, i, p) => o.ZwEnemyShips = ((DataStore.Premitives.List<DataStore.Premitives.StoragePremitive>)i).Convert(x => new ShipInfo(x, p)).ToArray()),
                    [1] = new HandlerInfo(
                        x => x.ZwOurShips.GetSerializationInfo(k => k.GetSerializationInfo()),
                        (o, i, p) => o.ZwOurShips = ((DataStore.Premitives.List<DataStore.Premitives.StoragePremitive>)i).Convert(x => new ShipInfo(x, p)).ToArray()),
                    [10] = new HandlerInfo(
                        x => x.ZwOpeningTorpedoAttack.GetSerializationInfo(k => k.GetSerializationInfo()),
                        (o, i, p) => o.ZwOpeningTorpedoAttack = ((DataStore.Premitives.List<DataStore.Premitives.StoragePremitive>)i).Convert(x => new TorpedoInfo(x, p)).ToArray()),
                    [11] = new HandlerInfo(
                        x => x.ZwBombardRound1.GetSerializationInfo(k => k.GetSerializationInfo()),
                        (o, i, p) => o.ZwBombardRound1 = ((DataStore.Premitives.List<DataStore.Premitives.StoragePremitive>)i).Convert(x => new BombardInfo(x, p)).ToArray()),
                    [12] = new HandlerInfo(
                        x => x.ZwBombardRound2.GetSerializationInfo(k => k.GetSerializationInfo()),
                        (o, i, p) => o.ZwBombardRound2 = ((DataStore.Premitives.List<DataStore.Premitives.StoragePremitive>)i).Convert(x => new BombardInfo(x, p)).ToArray()),
                    [13] = new HandlerInfo(
                        x => x.ZwBombardRound3.GetSerializationInfo(k => k.GetSerializationInfo()),
                        (o, i, p) => o.ZwBombardRound3 = ((DataStore.Premitives.List<DataStore.Premitives.StoragePremitive>)i).Convert(x => new BombardInfo(x, p)).ToArray()),
                    [14] = new HandlerInfo(
                        x => x.ZwClosingTorpedoAttack.GetSerializationInfo(k => k.GetSerializationInfo()),
                        (o, i, p) => o.ZwClosingTorpedoAttack = ((DataStore.Premitives.List<DataStore.Premitives.StoragePremitive>)i).Convert(x => new TorpedoInfo(x, p)).ToArray()),
                    [15] = new HandlerInfo(
                        x => x.ZwOurShipBattleEndHp.GetSerializationInfo(k => k.GetSerializationInfo()),
                        (o, i, p) => o.ZwOurShipBattleEndHp = ((DataStore.Premitives.List<DataStore.Premitives.StoragePremitive>)i).Convert(x => new ShipHpStatus(x, p)).ToArray()),
                    [16] = new HandlerInfo(
                        x => x.ZwEnemyShipBattleEndHp.GetSerializationInfo(k => k.GetSerializationInfo()),
                        (o, i, p) => o.ZwEnemyShipBattleEndHp = ((DataStore.Premitives.List<DataStore.Premitives.StoragePremitive>)i).Convert(x => new ShipHpStatus(x, p)).ToArray()),
                };
            }
        }

        internal BattleStatus() { }
        internal BattleStatus(DataStore.Premitives.StoragePremitive x, LinkedList<object> path) : base(x, path) { }

        object ICloneable.Clone()
        {
            var clone = (BattleStatus)MemberwiseClone();
            clone.ZwEnemyShips = ZwEnemyShips.DeepCloneArray();
            clone.ZwOurShips = ZwOurShips.DeepCloneArray();
            clone.ZwAirWarfare = ZwAirWarfare.Clone();
            clone.ZwNightWar = ZwNightWar.Clone();
            clone.ZwOpeningTorpedoAttack = ZwOpeningTorpedoAttack.DeepCloneArray().ForEach(x => x._parent = clone);
            clone.ZwBombardRound1 = ZwBombardRound1.DeepCloneArray().ForEach(xx => xx._parent = clone);
            clone.ZwBombardRound2 = ZwBombardRound1.DeepCloneArray().ForEach(xx => xx._parent = clone);
            clone.ZwBombardRound3 = ZwBombardRound1.DeepCloneArray().ForEach(xx => xx._parent = clone);
            clone.ZwClosingTorpedoAttack = ZwClosingTorpedoAttack.DeepCloneArray().ForEach(x => x._parent = clone);
            clone.ZwSupport = ZwSupport.Clone();

            clone.ZwSupport.ZwAttackInfo._parent = clone;
            clone.ZwAirWarfare.ZwOurStage3Report.ForEach(x => x._parent = clone);
            clone.ZwAirWarfare.ZwEnemyStage3Report.ForEach(x => x._parent = clone);
            return clone;
        }

        public partial class ShipInfo : AbstractDSSerializable<ShipInfo>, ICloneable
        {
            protected override IDictionary<ulong, HandlerInfo> CustomFieldHandlers
            {
                get
                {
                    return new Dictionary<ulong, HandlerInfo>() {
                        [9] = new HandlerInfo(
                            x => x.ZwEquipts.GetSerializationInfo(k => k.GetSerializationInfo()),
                            (o, i, p) => o.ZwEquipts = ((DataStore.Premitives.List<DataStore.Premitives.StoragePremitive>)i).Convert(x => new EquiptInfo(x, p)).ToArray()),
                    };
                }
            }

            internal ShipInfo() { }
            internal ShipInfo(DataStore.Premitives.StoragePremitive x, LinkedList<object> path) : base(x, path) { }

            object ICloneable.Clone()
            {
                var clone = (ShipInfo)MemberwiseClone();
                clone.ZwEquipts = ZwEquipts.DeepCloneArray();
                clone.ZwParameter = Parameter.Clone();
                clone.ZwEnhancement = Enhancement.Clone();
                return clone;
            }

            public partial class ParameterInfo : AbstractDSSerializable<ShipInfo>, ICloneable
            {
                internal ParameterInfo() { }
                internal ParameterInfo(DataStore.Premitives.StoragePremitive x, LinkedList<object> path) : base(x, path) { }

                object ICloneable.Clone() { return MemberwiseClone(); }
            }
        }

        public partial class NightWarInfo : AbstractDSSerializable<NightWarInfo>, ICloneable, IShipInfoHolder
        {
            protected override IDictionary<ulong, HandlerInfo> CustomFieldHandlers
            {
                get
                {
                    return new Dictionary<ulong, HandlerInfo>() {
                        [0] = new HandlerInfo(
                            x => x.ZwEnemyShips.GetSerializationInfo(k => k.GetSerializationInfo()),
                            (o, i, p) => o.ZwEnemyShips = ((DataStore.Premitives.List<DataStore.Premitives.StoragePremitive>)i).Convert(x => new ShipInfo(x, p)).ToArray()),
                        [1] = new HandlerInfo(
                            x => x.ZwOurShips.GetSerializationInfo(k => k.GetSerializationInfo()),
                            (o, i, p) => o.ZwOurShips = ((DataStore.Premitives.List<DataStore.Premitives.StoragePremitive>)i).Convert(x => new ShipInfo(x, p)).ToArray()),
                        [2] = new HandlerInfo(
                            x => x.ZwBombard.GetSerializationInfo(k => k.GetSerializationInfo()),
                            (o, i, p) => o.ZwBombard = ((DataStore.Premitives.List<DataStore.Premitives.StoragePremitive>)i).Convert(x => new BombardInfo(x, p)).ToArray()),
                    };
                }
            }
            
            internal NightWarInfo(DataStore.Premitives.StoragePremitive x, LinkedList<object> path) : base(x, path) { _parent = path.FirstOrDefault(p => p is IShipInfoHolder) as IShipInfoHolder; }

            object ICloneable.Clone()
            {
                var clone = (NightWarInfo)MemberwiseClone();
                clone.ZwEnemyShips = ZwEnemyShips.DeepCloneArray();
                clone.ZwOurShips = ZwOurShips.DeepCloneArray();
                clone.ZwBombard = ZwBombard.DeepCloneArray().ForEach(x => x._parent = clone);
                return clone;
            }
        }

        public partial class AirWarfareInfo : AbstractDSSerializable<AirWarfareInfo>, ICloneable
        {
            protected override IDictionary<ulong, HandlerInfo> CustomFieldHandlers
            {
                get
                {
                    return new Dictionary<ulong, HandlerInfo>() {
                        [1] = new HandlerInfo(
                            x => x.ZwOurCarrierShip.GetSerializationInfo(k => new DataStore.Premitives.SignedInteger(k)),
                            (o, i, p) => o.ZwOurCarrierShip = ((DataStore.Premitives.List<DataStore.Premitives.SignedInteger>)i).Convert(x => (int)x.Value).ToArray()),
                        [8] = new HandlerInfo(
                            x => x.ZwEnemyCarrierShip.GetSerializationInfo(k => new DataStore.Premitives.SignedInteger(k)),
                            (o, i, p) => o.ZwEnemyCarrierShip = ((DataStore.Premitives.List<DataStore.Premitives.SignedInteger>)i).Convert(x => (int)x.Value).ToArray()),
                        [15] = new HandlerInfo(
                            x => x.ZwOurShipBombed.GetSerializationInfo(k => new DataStore.Premitives.SignedInteger(k ? 1 : 0)),
                            (o, i, p) => o.ZwOurShipBombed = ((DataStore.Premitives.List<DataStore.Premitives.SignedInteger>)i).Convert(x => x.Value != 0).ToArray()),
                        [16] = new HandlerInfo(
                            x => x.ZwOurShipTorpedoed.GetSerializationInfo(k => new DataStore.Premitives.SignedInteger(k ? 1 : 0)),
                            (o, i, p) => o.ZwOurShipTorpedoed = ((DataStore.Premitives.List<DataStore.Premitives.SignedInteger>)i).Convert(x => x.Value != 0).ToArray()),
                        [17] = new HandlerInfo(
                            x => x.ZwOurShipDamages.GetSerializationInfo(k => new DataStore.Premitives.Double(k)),
                            (o, i, p) => o.ZwOurShipDamages = ((DataStore.Premitives.List<DataStore.Premitives.Double>)i).Convert(x => x.Value).ToArray()),
                        [18] = new HandlerInfo(
                            x => x.ZwEnemyShipBombed.GetSerializationInfo(k => new DataStore.Premitives.SignedInteger(k ? 1 : 0)),
                            (o, i, p) => o.ZwEnemyShipBombed = ((DataStore.Premitives.List<DataStore.Premitives.SignedInteger>)i).Convert(x => x.Value != 0).ToArray()),
                        [19] = new HandlerInfo(
                            x => x.ZwEnemyShipTorpedoed.GetSerializationInfo(k => new DataStore.Premitives.SignedInteger(k ? 1 : 0)),
                            (o, i, p) => o.ZwEnemyShipTorpedoed = ((DataStore.Premitives.List<DataStore.Premitives.SignedInteger>)i).Convert(x => x.Value != 0).ToArray()),
                        [20] = new HandlerInfo(
                            x => x.ZwEnemyShipDamages.GetSerializationInfo(k => new DataStore.Premitives.Double(k)),
                            (o, i, p) => o.ZwEnemyShipDamages = ((DataStore.Premitives.List<DataStore.Premitives.Double>)i).Convert(x => x.Value).ToArray()),
                        [23] = new HandlerInfo(
                            x => x.ZwCutInEquipts.GetSerializationInfo(k => k.GetSerializationInfo()),
                            (o, i, p) => o.ZwCutInEquipts = ((DataStore.Premitives.List<DataStore.Premitives.StoragePremitive>)i).Convert(x => new EquiptInfo(x, p)).ToArray()),
                        [24] = new HandlerInfo(
                            x => x.ZwOurStage3Report.GetSerializationInfo(k => k.GetSerializationInfo()),
                            (o, i, p) => o.ZwOurStage3Report = ((DataStore.Premitives.List<DataStore.Premitives.StoragePremitive>)i).Convert(x => new Stage3Report(x, p)).ToArray()),
                        [25] = new HandlerInfo(
                            x => x.ZwEnemyStage3Report.GetSerializationInfo(k => k.GetSerializationInfo()),
                            (o, i, p) => o.ZwEnemyStage3Report = ((DataStore.Premitives.List<DataStore.Premitives.StoragePremitive>)i).Convert(x => new Stage3Report(x, p)).ToArray()),
                    };
                }
            }
            
            internal AirWarfareInfo(DataStore.Premitives.StoragePremitive x, LinkedList<object> path) : base(x, path) { _parent = path.FirstOrDefault(p => p is IShipInfoHolder) as IShipInfoHolder; }

            object ICloneable.Clone()
            {
                var clone = (AirWarfareInfo)MemberwiseClone();
                clone.ZwOurCarrierShip = (int[])ZwOurCarrierShip.Clone();
                clone.ZwOurShipBombed = (bool[])ZwOurShipBombed.Clone();
                clone.ZwOurShipTorpedoed = (bool[])ZwOurShipTorpedoed.Clone();
                clone.ZwOurShipDamages = (double[])ZwOurShipDamages.Clone();
                clone.ZwEnemyCarrierShip = (int[])ZwEnemyCarrierShip.Clone();
                clone.ZwEnemyShipBombed = (bool[])ZwEnemyShipBombed.Clone();
                clone.ZwEnemyShipTorpedoed = (bool[])ZwEnemyShipTorpedoed.Clone();
                clone.ZwEnemyShipDamages = (double[])ZwEnemyShipDamages.Clone();
                clone.ZwCutInEquipts = ZwCutInEquipts.DeepCloneArray();
                clone.ZwEnemyStage3Report = ZwEnemyStage3Report.DeepCloneArray();
                clone.ZwOurStage3Report = ZwOurStage3Report.DeepCloneArray();
                return clone;
            }

            public partial class Stage3Report : AbstractDSSerializable<Stage3Report>, ICloneable
            {
                internal Stage3Report() { }
                internal Stage3Report(DataStore.Premitives.StoragePremitive x, LinkedList<object> path) : base(x, path) { _parent = path.FirstOrDefault(p => p is IShipInfoHolder) as IShipInfoHolder; }

                object ICloneable.Clone() { return MemberwiseClone(); }
            }
        }

        public partial class TorpedoInfo : AbstractDSSerializable<TorpedoInfo>, ICloneable
        {
            internal TorpedoInfo(DataStore.Premitives.StoragePremitive x, LinkedList<object> path) : base(x, path) { _parent = path.FirstOrDefault(p => p is IShipInfoHolder) as IShipInfoHolder; }

            object ICloneable.Clone() { return MemberwiseClone(); }
        }

        public partial class BombardInfo : AbstractDSSerializable<BombardInfo>, ICloneable
        {
            protected override IDictionary<ulong, HandlerInfo> CustomFieldHandlers
            {
                get
                {
                    return new Dictionary<ulong, HandlerInfo>() {
                        [1] = new HandlerInfo(
                            x => x.ZwTo.GetSerializationInfo(k => new DataStore.Premitives.SignedInteger(k)),
                            (o, i, p) => o.ZwTo = ((DataStore.Premitives.List<DataStore.Premitives.SignedInteger>)i).Convert(x => (int)x.Value).ToArray()),
                        [3] = new HandlerInfo(
                            x => x.ZwDamage.GetSerializationInfo(k => new DataStore.Premitives.Double(k)),
                            (o, i, p) => o.ZwDamage = ((DataStore.Premitives.List<DataStore.Premitives.Double>)i).Convert(x => x.Value).ToArray()),
                        [4] = new HandlerInfo(
                            x => x.ZwEquipts.GetSerializationInfo(k => k.GetSerializationInfo()),
                            (o, i, p) => o.ZwEquipts = ((DataStore.Premitives.List<DataStore.Premitives.StoragePremitive>)i).Convert(x => new EquiptInfo(x, p)).ToArray()),
                    };
                }
            }
            internal BombardInfo(DataStore.Premitives.StoragePremitive x, LinkedList<object> path) : base(x, path) { _parent = path.FirstOrDefault(p => p is IShipInfoHolder) as IShipInfoHolder; }

            object ICloneable.Clone()
            {
                var clone = (BombardInfo)MemberwiseClone();
                clone.ZwTo = (int[])ZwTo.Clone();
                clone.ZwEquipts = ZwEquipts.DeepCloneArray();
                clone.ZwDamage = (double[])ZwDamage.Clone();
                return clone;
            }
        }

        public partial class SupportInfo : AbstractDSSerializable<SupportInfo>, ICloneable
        {
            protected override IDictionary<ulong, HandlerInfo> CustomFieldHandlers
            {
                get
                {
                    return new Dictionary<ulong, HandlerInfo>() {
                        [0] = new HandlerInfo(
                            x => x.ZwSupportShips.GetSerializationInfo(k => k.GetSerializationInfo()),
                            (o, i, p) => o.ZwSupportShips = ((DataStore.Premitives.List<DataStore.Premitives.StoragePremitive>)i).Convert(x => new ShipInfo(x, p)).ToArray()),
                    };
                }
            }

            internal SupportInfo() { }
            internal SupportInfo(DataStore.Premitives.StoragePremitive x, LinkedList<object> path) : base(x, path) { }

            object ICloneable.Clone()
            {
                SupportInfo mwc = (SupportInfo)MemberwiseClone();
                mwc.ZwSupportShips = ZwSupportShips.DeepCloneArray();
                mwc.ZwAttackInfo = AttackInfo.Clone();
                return mwc;
            }
        }

        public partial class ShipHpStatus : AbstractDSSerializable<ShipHpStatus>, ICloneable
        {
            internal ShipHpStatus(DataStore.Premitives.StoragePremitive x, LinkedList<object> path) : base(x, path) { _parent = path.FirstOrDefault(p => p is BattleStatus) as BattleStatus; }

            object ICloneable.Clone() { return MemberwiseClone(); }
        }
    }
}
