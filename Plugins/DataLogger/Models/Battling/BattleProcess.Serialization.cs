﻿using LynLogger.DataStore.Extensions;
using LynLogger.DataStore.MasterInfo;
using LynLogger.DataStore.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LynLogger.Utilities;
using LynLogger.DataStore.Premitives;

namespace LynLogger.Models.Battling
{
    public partial class BattleProcess : AbstractDSSerializable<BattleProcess>, ICloneable, IShipInfoHolder
    {
        protected override IDictionary<ulong, HandlerInfo> CustomFieldHandlers
        {
            get
            {
                return new Dictionary<ulong, HandlerInfo>() {
                    [0] = new HandlerInfo(
                        (x, p) => x.ZwEnemyShips.GetSerializationInfo(p),
                        (o, i, p) => o.ZwEnemyShips = ((DsList<StoragePremitive>)i).Convert(x => new ShipInfo(x, p)).ToArray()),
                    [1] = new HandlerInfo(
                        (x, p) => x.ZwOurShips.GetSerializationInfo(p),
                        (o, i, p) => o.ZwOurShips = ((DsList<StoragePremitive>)i).Convert(x => new ShipInfo(x, p)).ToArray()),
                    [10] = new HandlerInfo(3,
                        (x, p) => x.ZwOpeningTorpedoAttack.GetSerializationInfo(p),
                        (o, i, p) => o.ZwOpeningTorpedoAttack = ((DsList<StoragePremitive>)i)?.Convert(x => new TorpedoInfo(x, p)).ToArray()),
                    [11] = new HandlerInfo(3,
                        (x, p) => x.ZwBombardRound1.GetSerializationInfo(p),
                        (o, i, p) => o.ZwBombardRound1 = ((DsList<StoragePremitive>)i)?.Convert(x => new BombardInfo(x, p)).ToArray()),
                    [12] = new HandlerInfo(3,
                        (x, p) => x.ZwBombardRound2.GetSerializationInfo(p),
                        (o, i, p) => o.ZwBombardRound2 = ((DsList<StoragePremitive>)i)?.Convert(x => new BombardInfo(x, p)).ToArray()),
                    [13] = new HandlerInfo(3,
                        (x, p) => x.ZwBombardRound3.GetSerializationInfo(p),
                        (o, i, p) => o.ZwBombardRound3 = ((DsList<StoragePremitive>)i)?.Convert(x => new BombardInfo(x, p)).ToArray()),
                    [14] = new HandlerInfo(3,
                        (x, p) => x.ZwClosingTorpedoAttack.GetSerializationInfo(p),
                        (o, i, p) => o.ZwClosingTorpedoAttack = ((DsList<StoragePremitive>)i)?.Convert(x => new TorpedoInfo(x, p)).ToArray()),
                    [15] = new HandlerInfo(
                        (x, p) => (p.Count > 3 ? x.OurShipBattleEndHp : x.ZwOurShipBattleEndHp).GetSerializationInfo(p),
                        (o, i, p) => o.ZwOurShipBattleEndHp = ((DsList<StoragePremitive>)i)?.Convert(x => new ShipHpStatus(x, p)).ToArray()),
                    [16] = new HandlerInfo(
                        (x, p) => (p.Count > 3 ? x.EnemyShipBattleEndHp : x.ZwEnemyShipBattleEndHp).GetSerializationInfo(p),
                        (o, i, p) => o.ZwEnemyShipBattleEndHp = ((DsList<StoragePremitive>)i)?.Convert(x => new ShipHpStatus(x, p)).ToArray()),
                };
            }
        }

        internal BattleProcess() { }
        internal BattleProcess(StoragePremitive x, LinkedList<object> path) : base(x, path) { }

        object ICloneable.Clone()
        {
            var clone = (BattleProcess)MemberwiseClone();
            clone.ZwEnemyShips = ZwEnemyShips.DeepCloneArray();
            clone.ZwOurShips = ZwOurShips.DeepCloneArray();
            clone.ZwAirWarfare = ZwAirWarfare.Clone();
            clone.ZwAirWarfare2 = ZwAirWarfare2.Clone();
            clone.ZwNightWar = ZwNightWar.Clone();
            clone.ZwSupport = ZwSupport.Clone();

            clone.ZwOpeningTorpedoAttack = ZwOpeningTorpedoAttack.DeepCloneArray().ForEach(x => x._parent = clone);
            clone.ZwBombardRound1 = ZwBombardRound1.DeepCloneArray().ForEach(xx => xx._parent = clone);
            clone.ZwBombardRound2 = ZwBombardRound2.DeepCloneArray().ForEach(xx => xx._parent = clone);
            clone.ZwBombardRound3 = ZwBombardRound3.DeepCloneArray().ForEach(xx => xx._parent = clone);
            clone.ZwClosingTorpedoAttack = ZwClosingTorpedoAttack.DeepCloneArray().ForEach(x => x._parent = clone);

            if(clone.ZwSupport != null)
                clone.ZwSupport.ZwAttackInfo._parent = clone;

            if(clone.NightWar != null)
                clone.NightWar._parent = clone;

            if (clone.ZwAirWarfare != null)
                clone.ZwAirWarfare._parent = clone;

            if (clone.ZwAirWarfare2 != null)
                clone.ZwAirWarfare2._parent = clone;
            
            clone.ZwOurShipBattleEndHp = null;
            clone.ZwEnemyShipBattleEndHp = null;

            return clone;
        }

        public partial class ShipInfo : AbstractDSSerializable<ShipInfo>, ICloneable
        {
            protected override IDictionary<ulong, HandlerInfo> CustomFieldHandlers
            {
                get
                {
                    return new Dictionary<ulong, HandlerInfo>() {
                        [1] = new HandlerInfo(
                            (x, p) => null,
                            (o, i, p) => (o.ZwShipNameType ?? (o.ZwShipNameType = new ShipNameType(0))).ShipId = (int)(SignedInteger)i, true),
                        [2] = new HandlerInfo(
                            (x, p) => null,
                            (o, i, p) => (o.ZwShipNameType ?? (o.ZwShipNameType = new ShipNameType(0))).TypeName = (DataStore.Premitives.DsString)i, true),
                        [3] = new HandlerInfo(
                            (x, p) => null,
                            (o, i, p) => (o.ZwShipNameType ?? (o.ZwShipNameType = new ShipNameType(0))).ShipName = (DataStore.Premitives.DsString)i, true),
                        [9] = new HandlerInfo(
                            (x, p) => x.ZwEquipts.GetSerializationInfo(p),
                            (o, i, p) => o.ZwEquipts = ((DsList<StoragePremitive>)i).Convert(x => new EquiptInfo(x, p)).ToArray()),
                    };
                }
            }

            internal ShipInfo() { }
            internal ShipInfo(StoragePremitive x, LinkedList<object> path) : base(x, path) { }

            object ICloneable.Clone()
            {
                var clone = (ShipInfo)MemberwiseClone();
                clone.ZwEquipts = ZwEquipts.DeepCloneArray();
                clone.ZwParameter = Parameter.Clone();
                clone.ZwEnhancement = Enhancement.Clone();
                return clone;
            }

            public partial class ParameterInfo : AbstractDSSerializable<ParameterInfo>, ICloneable
            {
                internal ParameterInfo() { }
                internal ParameterInfo(StoragePremitive x, LinkedList<object> path) : base(x, path) { }

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
                            (x, p) => x.ZwEnemyShips.GetSerializationInfo(p),
                            (o, i, p) => o.ZwEnemyShips = ((DsList<StoragePremitive>)i)?.Convert(x => new ShipInfo(x, p)).ToArray()),
                        [1] = new HandlerInfo(
                            (x, p) => x.ZwOurShips.GetSerializationInfo(p),
                            (o, i, p) => o.ZwOurShips = ((DsList<StoragePremitive>)i)?.Convert(x => new ShipInfo(x, p)).ToArray()),
                        [2] = new HandlerInfo(
                            (x, p) => x.ZwBombard.GetSerializationInfo(p),
                            (o, i, p) => o.ZwBombard = ((DsList<StoragePremitive>)i)?.Convert(x => new BombardInfo(x, p)).ToArray()),
                    };
                }
            }
            
            internal NightWarInfo(StoragePremitive x, LinkedList<object> path) : base(x, path) { _parent = path.FirstOrDefault(p => p is IShipInfoHolder) as IShipInfoHolder; }

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
                            (x, p) => x.ZwOurCarrierShip.GetSerializationInfo(p, (k, p1) => new SignedInteger(k)),
                            (o, i, p) => o.ZwOurCarrierShip = ((DsList<SignedInteger>)i).Convert(x => (int)x.Value).ToArray()),
                        [8] = new HandlerInfo(
                            (x, p) => x.ZwEnemyCarrierShip.GetSerializationInfo(p, (k, p1) => new SignedInteger(k)),
                            (o, i, p) => o.ZwEnemyCarrierShip = ((DsList<SignedInteger>)i).Convert(x => (int)x.Value).ToArray()),
                        [15] = new HandlerInfo(
                            (x, p) => x.ZwOurShipBombed.GetSerializationInfo(p, (k, p1) => new SignedInteger(k ? 1 : 0)),
                            (o, i, p) => o.ZwOurShipBombed = ((DsList<SignedInteger>)i).Convert(x => x.Value != 0).ToArray()),
                        [16] = new HandlerInfo(
                            (x, p) => x.ZwOurShipTorpedoed.GetSerializationInfo(p, (k, p1) => new SignedInteger(k ? 1 : 0)),
                            (o, i, p) => o.ZwOurShipTorpedoed = ((DsList<SignedInteger>)i).Convert(x => x.Value != 0).ToArray()),
                        [17] = new HandlerInfo(
                            (x, p) => x.ZwOurShipDamages.GetSerializationInfo(p, (k, p1) => new SignedInteger(k)),
                            (o, i, p) => o.ZwOurShipDamages = (i is DsList<DsDouble> ? ((DsList<DsDouble>)i).Convert(x => (int)x.Value) : ((DsList<SignedInteger>)i).Convert(x => (int)x.Value)).ToArray()),
                        [18] = new HandlerInfo(
                            (x, p) => x.ZwEnemyShipBombed.GetSerializationInfo(p, (k, p1) => new SignedInteger(k ? 1 : 0)),
                            (o, i, p) => o.ZwEnemyShipBombed = ((DsList<SignedInteger>)i).Convert(x => x.Value != 0).ToArray()),
                        [19] = new HandlerInfo(
                            (x, p) => x.ZwEnemyShipTorpedoed.GetSerializationInfo(p, (k, p1) => new SignedInteger(k ? 1 : 0)),
                            (o, i, p) => o.ZwEnemyShipTorpedoed = ((DsList<SignedInteger>)i).Convert(x => x.Value != 0).ToArray()),
                        [20] = new HandlerInfo(
                            (x, p) => x.ZwEnemyShipDamages.GetSerializationInfo(p, (k, p1) => new SignedInteger(k)),
                            (o, i, p) => o.ZwEnemyShipDamages = (i is DsList<DsDouble> ? ((DsList<DsDouble>)i).Convert(x => (int)x.Value) : ((DsList<SignedInteger>)i).Convert(x => (int)x.Value)).ToArray()),
                        [23] = new HandlerInfo(
                            (x, p) => x.ZwCutInEquipts.GetSerializationInfo(p),
                            (o, i, p) => o.ZwCutInEquipts = ((DsList<StoragePremitive>)i)?.Convert(x => new EquiptInfo(x, p)).ToArray()),
                        [24] = new HandlerInfo(
                            (x, p) => x.ZwOurStage3Report.GetSerializationInfo(p),
                            (o, i, p) => o.ZwOurStage3Report = ((DsList<StoragePremitive>)i)?.Convert(x => new Stage3Report(x, p)).ToArray()),
                        [25] = new HandlerInfo(
                            (x, p) => x.ZwEnemyStage3Report.GetSerializationInfo(p),
                            (o, i, p) => o.ZwEnemyStage3Report = ((DsList<StoragePremitive>)i)?.Convert(x => new Stage3Report(x, p)).ToArray()),
                    };
                }
            }
            
            internal AirWarfareInfo(StoragePremitive x, LinkedList<object> path) : base(x, path) { _parent = path.FirstOrDefault(p => p is IShipInfoHolder) as IShipInfoHolder; }

            object ICloneable.Clone()
            {
                var clone = (AirWarfareInfo)MemberwiseClone();
                clone.ZwOurCarrierShip = (int[])ZwOurCarrierShip.Clone();
                clone.ZwOurShipBombed = (bool[])ZwOurShipBombed.Clone();
                clone.ZwOurShipTorpedoed = (bool[])ZwOurShipTorpedoed.Clone();
                clone.ZwOurShipDamages = (int[])ZwOurShipDamages.Clone();
                clone.ZwEnemyCarrierShip = (int[])ZwEnemyCarrierShip.Clone();
                clone.ZwEnemyShipBombed = (bool[])ZwEnemyShipBombed.Clone();
                clone.ZwEnemyShipTorpedoed = (bool[])ZwEnemyShipTorpedoed.Clone();
                clone.ZwEnemyShipDamages = (int[])ZwEnemyShipDamages.Clone();
                clone.ZwCutInEquipts = ZwCutInEquipts.DeepCloneArray();
                clone.ZwEnemyStage3Report = null;
                clone.ZwOurStage3Report = null;
                return clone;
            }

            public partial class Stage3Report : AbstractDSSerializable<Stage3Report>, ICloneable
            {
                internal Stage3Report() { }
                internal Stage3Report(StoragePremitive x, LinkedList<object> path) : base(x, path) { _parent = path.FirstOrDefault(p => p is IShipInfoHolder) as IShipInfoHolder; }

                object ICloneable.Clone() { return MemberwiseClone(); }
            }
        }

        public partial class TorpedoInfo : AbstractDSSerializable<TorpedoInfo>, ICloneable
        {
            protected override IDictionary<ulong, HandlerInfo> CustomFieldHandlers
            {
                get
                {
                    return new Dictionary<ulong, HandlerInfo>() {
                        [2] = new HandlerInfo(
                            (x, p) => (SignedInteger)x.ZwDamage,
                            (o, i, p) => o.ZwDamage = (i is DsDouble ? (int)(i as DsDouble).Value : (int)(i as SignedInteger).Value)),
                    };
                }
            }

            internal TorpedoInfo(StoragePremitive x, LinkedList<object> path) : base(x, path) { _parent = path.FirstOrDefault(p => p is IShipInfoHolder) as IShipInfoHolder; }
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
                            (x, p) => x.ZwTo.GetSerializationInfo(p, (k, p1) => new SignedInteger(k)),
                            (o, i, p) => o.ZwTo = ((DsList<SignedInteger>)i).Convert(x => (int)x.Value).ToArray()),
                        [3] = new HandlerInfo(
                            (x, p) => x.ZwDamage.GetSerializationInfo(p, (k, p1) => new SignedInteger(k)),
                            (o, i, p) => o.ZwDamage = (i is DsList<DsDouble> ? ((DsList<DsDouble>)i).Convert(x => (int)x.Value) : ((DsList<SignedInteger>)i).Convert(x => (int)x.Value)).ToArray()),
                        [4] = new HandlerInfo(
                            (x, p) => x.ZwEquipts.GetSerializationInfo(p),
                            (o, i, p) => o.ZwEquipts = ((DsList<StoragePremitive>)i).Convert(x => new EquiptInfo(x, p)).ToArray()),
                    };
                }
            }
            internal BombardInfo(StoragePremitive x, LinkedList<object> path) : base(x, path) { _parent = path.FirstOrDefault(p => p is IShipInfoHolder) as IShipInfoHolder; }

            object ICloneable.Clone()
            {
                var clone = (BombardInfo)MemberwiseClone();
                clone.ZwTo = (int[])ZwTo.Clone();
                clone.ZwEquipts = ZwEquipts.DeepCloneArray();
                clone.ZwDamage = (int[])ZwDamage.Clone();
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
                            (x, p) => x.ZwSupportShips.GetSerializationInfo(p),
                            (o, i, p) => o.ZwSupportShips = ((DsList<StoragePremitive>)i).Convert(x => new ShipInfo(x, p)).ToArray()),
                    };
                }
            }

            internal SupportInfo() { }
            internal SupportInfo(StoragePremitive x, LinkedList<object> path) : base(x, path) { }

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
            internal ShipHpStatus(StoragePremitive x, LinkedList<object> path) : base(x, path) { _parent = path.FirstOrDefault(p => p is BattleProcess) as BattleProcess; }

            object ICloneable.Clone() { return MemberwiseClone(); }
        }
    }
}
