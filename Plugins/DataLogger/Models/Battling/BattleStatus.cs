using Grabacr07.KanColleWrapper.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace LynLogger.Models.Battling
{
    public interface IShipInfoHolder
    {
        IReadOnlyList<BattleStatus.ShipInfo> EnemyShips { get; }
        IReadOnlyList<BattleStatus.ShipInfo> OurShips { get; }
    }

    public class BattleStatus : ICloneable, IShipInfoHolder
    {
        internal ShipInfo[] ZwEnemyShips;
        internal ShipInfo[] ZwOurShips;
        internal Formation ZwOurFormation;
        internal Formation ZwEnemyFormation;
        internal EncounterForm ZwEncounter;
        internal AirWarfareInfo ZwAirWarfare;
        internal bool ZwHasNightWar;
        internal NightWarInfo ZwNightWar;
        internal ReconnResult ZwOurReconn;
        internal ReconnResult ZwEnemyReconn;
        internal TorpedoInfo[] ZwOpeningTorpedoAttack;
        internal BombardInfo[][] ZwBombards;
        internal TorpedoInfo[] ZwClosingTorpedoAttack;
        internal string ZwRawData;

        public IReadOnlyList<ShipInfo> EnemyShips { get { return ZwEnemyShips; } }
        public IReadOnlyList<ShipInfo> OurShips { get { return ZwOurShips; } }
        public Formation OurFormation { get { return ZwOurFormation; } }
        public Formation EnemyFormation { get { return ZwEnemyFormation; } }
        public EncounterForm Encounter { get { return ZwEncounter; } }
        public AirWarfareInfo AirWarfare { get { return ZwAirWarfare; } }
        public ReconnResult OurReconn { get { return ZwOurReconn; } }
        public ReconnResult EnemyReconn { get { return ZwEnemyReconn; } }
        public IReadOnlyList<TorpedoInfo> OpeningTorpedoAttack { get { return ZwOpeningTorpedoAttack ?? (ZwOpeningTorpedoAttack = new TorpedoInfo[0]); } }
        public IReadOnlyList<IReadOnlyList<BombardInfo>> Bombards { get { return ZwBombards ?? (ZwBombards = new BombardInfo[0][]); } }
        public IReadOnlyList<TorpedoInfo> ClosingTorpedoAttack { get { return ZwClosingTorpedoAttack ?? (ZwClosingTorpedoAttack = new TorpedoInfo[0]); } }
        public string RawData { get { return ZwRawData; } }
        public bool HasNightWar { get { return ZwHasNightWar; } }
        public NightWarInfo NightWar
        {
            get { return ZwNightWar; }
            set
            {
                if(ZwNightWar == null && HasNightWar) {
                    ZwNightWar = value.Clone();
                    ZwNightWar._parent = this;
                } else {
                    throw new InvalidOperationException();
                }
            }
        }

        public IEnumerable<ShipHpStatus> OurShipBattleEndHp { get { return OurShips.Select(x => new ShipHpStatus(x)).Select(x => x.ProcessBattle(this)); } }
        public IEnumerable<ShipHpStatus> EnemyShipBattleEndHp { get { return EnemyShips.Select(x => new ShipHpStatus(x)).Select(x => x.ProcessBattle(this)); } }

        public class NightWarInfo : ICloneable, IShipInfoHolder
        {
            internal ShipInfo[] ZwEnemyShips;
            internal ShipInfo[] ZwOurShips;
            internal BombardInfo[] ZwBombard;
            internal string ZwOurReconnInTouchName;
            internal int ZwOurReconnInTouch;
            internal string ZwEnemyReconnInTouchName;
            internal int ZwEnemyReconnInTouch;
            internal string ZwRawData;
            internal IShipInfoHolder _parent;

            public IReadOnlyList<ShipInfo> EnemyShips { get { return _parent?.EnemyShips ?? ZwEnemyShips; } }
            public IReadOnlyList<ShipInfo> OurShips { get { return _parent?.OurShips ?? ZwOurShips; } }
            public IReadOnlyList<BombardInfo> Bombard { get { return ZwBombard ?? (ZwBombard = new BombardInfo[0]); } }
            public int OurReconnInTouch { get { return ZwOurReconnInTouch; } }
            public string OurReconnInTouchName { get { return ZwOurReconnInTouchName; } }
            public int EnemyReconnInTouch { get { return ZwEnemyReconnInTouch; } }
            public string EnemyReconnInTouchName { get { return ZwEnemyReconnInTouchName; } }
            public string RawData { get { return ZwRawData; } }

            object ICloneable.Clone()
            {
                var clone = (NightWarInfo)MemberwiseClone();
                clone.ZwEnemyShips = ZwEnemyShips.DeepCloneArray();
                clone.ZwOurShips = ZwOurShips.DeepCloneArray();
                clone.ZwBombard = ZwBombard.DeepCloneArray().ForEach(x => x._parent = clone);
                return clone;
            }
        }

        public class ShipInfo : ICloneable
        {
            internal int ZwId;
            internal int ZwShipId;
            internal string ZwShipTypeName;
            internal string ZwShipName;
            internal int ZwLv;
            internal int ZwCurrentHp;
            internal int ZwMaxHp;
            internal ParameterInfo ZwParameter;
            internal ParameterInfo ZwEnhancement;
            internal EquiptInfo[] ZwEquipts;

            public int Id { get { return ZwId; } }
            public int ShipId { get { return ZwShipId; } }
            public string ShipTypeName { get { return ZwShipTypeName; } }
            public string ShipName { get { return ZwShipName; } }
            public int Lv { get { return ZwLv; } }
            public int CurrentHp { get { return ZwCurrentHp; } }
            public int MaxHp { get { return ZwMaxHp; } }
            public ParameterInfo Parameter { get { return ZwParameter; } }
            public ParameterInfo Enhancement { get { return ZwEnhancement; } }
            public IReadOnlyList<EquiptInfo> Equipts { get { return ZwEquipts; } }
            public int ShipAsControl { get { return Equipts.Sum(x => x.SlotAsControl); } }

            public struct ParameterInfo
            {
                internal int ZwPower;
                internal int ZwTorpedo;
                internal int ZwAntiAir;
                internal int ZwDefense;

                public int Power { get { return ZwPower; } }
                public int Torpedo { get { return ZwTorpedo; } }
                public int AntiAir { get { return ZwAntiAir; } }
                public int Defense { get { return ZwDefense; } }
            }

            object ICloneable.Clone()
            {
                var clone = (ShipInfo)MemberwiseClone();
                clone.ZwEquipts = (EquiptInfo[])ZwEquipts.Clone();
                return clone;
            }
        }

        public class AirWarfareInfo : ICloneable
        {
            internal AirspaceControl ZwOurAirspaceControl;

            internal int[] ZwOurCarrierShip;
            internal int ZwOurStage1Engaged;
            internal int ZwOurStage1Lost;
            internal int ZwOurReconnInTouch;
            internal int ZwOurStage2Engaged;
            internal int ZwOurStage2Lost;
            internal string ZwOurReconnInTouchName;

            internal int[] ZwEnemyCarrierShip;
            internal int ZwEnemyStage1Engaged;
            internal int ZwEnemyStage1Lost;
            internal int ZwEnemyReconnInTouch;
            internal int ZwEnemyStage2Engaged;
            internal int ZwEnemyStage2Lost;
            internal string ZwEnemyReconnInTouchName;

            internal bool[] ZwOurShipBombed;
            internal bool[] ZwOurShipTorpedoed;
            internal double[] ZwOurShipDamages;

            internal bool[] ZwEnemyShipBombed;
            internal bool[] ZwEnemyShipTorpedoed;
            internal double[] ZwEnemyShipDamages;

            internal int ZwCutInShipNo = -1;
            internal AaCutInType ZwCutInType;
            internal SimpleEquiptInfo[] ZwCutInEquipts;

            public AirspaceControl OurAirspaceControl { get { return ZwOurAirspaceControl; } }
            public AirspaceControl EnemyAirspaceControl { get { return (AirspaceControl)(6 - (int)ZwOurAirspaceControl); } }

            public IEnumerable<ShipInfo> OurCarrierShip { get { return ZwOurCarrierShip.Select(x => _parent.OurShips[x]); } }
            public int OurStage1Engaged { get { return ZwOurStage1Engaged; } }
            public int OurStage1Lost { get { return ZwOurStage1Lost; } }
            public int OurReconnInTouch { get { return ZwOurReconnInTouch; } }
            public int OurStage2Engaged { get { return ZwOurStage2Engaged; } }
            public int OurStage2Lost { get { return ZwOurStage2Lost; } }
            public string OurReconnInTouchName { get { return ZwOurReconnInTouchName; } }

            public IEnumerable<ShipInfo> EnemyCarrierShip { get { return ZwEnemyCarrierShip.Select(x => _parent.EnemyShips[x]); } }
            public int EnemyStage1Engaged { get { return ZwEnemyStage1Engaged; } }
            public int EnemyStage1Lost { get { return ZwEnemyStage1Lost; } }
            public int EnemyReconnInTouch { get { return ZwEnemyReconnInTouch; } }
            public int EnemyStage2Engaged { get { return ZwEnemyStage2Engaged; } }
            public int EnemyStage2Lost { get { return ZwEnemyStage2Lost; } }
            public string EnemyReconnInTouchName { get { return ZwEnemyReconnInTouchName; } }

            public ShipInfo CutInShip { get { return (ZwCutInShipNo < 0 || ZwCutInShipNo > 5) ? null : _parent.OurShips[ZwCutInShipNo]; } }
            public AaCutInType CutInType { get { return ZwCutInType; } }
            public IReadOnlyList<SimpleEquiptInfo> CutInEquipts { get { return ZwCutInEquipts; } }

            public int OurAsControlValue { get { return OurCarrierShip.Sum(x => x.ShipAsControl); } }
            public int EnemyAsControlValue { get { return EnemyCarrierShip.Sum(x => x.ShipAsControl); } }

            //public IReadOnlyList<bool> OurShipBombed { get { return ZwOurShipBombed; } }
            //public IReadOnlyList<bool> OurShipTorpedoed { get { return ZwOurShipTorpedoed; } }
            //public IReadOnlyList<double> OurShipDamages { get { return ZwOurShipDamages; } }

            //public IReadOnlyList<bool> EnemyShipBombed { get { return ZwEnemyShipBombed; } }
            //public IReadOnlyList<bool> EnemyShipTorpedoed { get { return ZwEnemyShipTorpedoed; } }
            //public IReadOnlyList<double> EnemyShipDamages { get { return ZwEnemyShipDamages; } }

            public IEnumerable<Stage3Report> OurStage3Report
            {
                get
                {
                    return Helpers.Zip(ZwOurShipDamages, ZwOurShipBombed, ZwOurShipTorpedoed, _parent.OurShips,
                        (damage, bombed, torpedoed, ship) =>
                            new Stage3Report() {
                                ZwShip = ship,
                                ZwDamage = damage,
                                ZwBombed = bombed,
                                ZwTorpedoed = torpedoed
                            }
                    ).Where(x => x.Bombed || x.Torpedoed);
                }
            }

            public IEnumerable<Stage3Report> EnemyStage3Report
            {
                get
                {
                    return Helpers.Zip(ZwEnemyShipDamages, ZwEnemyShipBombed, ZwEnemyShipTorpedoed, _parent.EnemyShips,
                        (damage, bombed, torpedoed, ship) =>
                            new Stage3Report() {
                                ZwShip = ship,
                                ZwDamage = damage,
                                ZwBombed = bombed,
                                ZwTorpedoed = torpedoed
                            }
                    ).Where(x => x.Bombed || x.Torpedoed);
                }
            }

            internal IShipInfoHolder _parent;
            public AirWarfareInfo(IShipInfoHolder parent) { _parent = parent; }

            public struct Stage3Report
            {
                internal ShipInfo ZwShip;
                internal bool ZwBombed;
                internal bool ZwTorpedoed;
                internal double ZwDamage;

                public ShipInfo Ship { get { return ZwShip; } }
                public bool Bombed { get { return ZwBombed; } }
                public bool Torpedoed { get { return ZwTorpedoed; } }
                public double Damage { get { return ZwDamage; } }
            }

            public enum AirspaceControl
            {
                [Description("未知")]
                None = 0,

                [Description("确保")]
                Supremacy = 1,

                [Description("优势")]
                Superiority = 2,

                [Description("均衡")]
                Parity = 3,

                [Description("劣势")]
                Denial = 4,

                [Description("丧失")]
                Incapability = 5,

                [Description("未知")]
                InvertNone = 6
            }

            public enum AaCutInType
            {
                [Description("无")]
                None = 0,

                [Description("秋月 / 双高角炮 / 有电探")]
                AkizukiDualNavalGunWithRadar = 1,

                [Description("秋月 / 高角炮 / 有电探")]
                AkizukiNavalGunWithRadar = 2,

                [Description("秋月 / 双高角炮 / 无电探")]
                AkizukiDualNavalGunNoRadar = 3,

                [Description("大口径主炮 / 三式弹 / 高射装置 / 有电探")]
                ArtilleryAaT3ShellWithRadar = 4,

                [Description("双高角炮+高射装置 / 有电探")]
                DualNavalAndAaGunWithRadar = 5,

                [Description("大口径主炮 / 三式弹 / 高射装置 / 无电探")]
                ArtilleryAaT3ShellNoRadar = 6,

                [Description("高角炮 / 高射装置 / 有电探")]
                NavalGunWithAaGunWithRadar = 7,

                [Description("高角炮+高射装置 / 有电探")]
                NavalAndAaGunWithRadar = 8,

                [Description("高角炮 / 高射装置 / 无电探")]
                NavalGunWithAaGunNoRadar = 9,

                [Description("摩耶改二 / 集中机枪 / 高角炮 / 有电探")]
                MayaGen2MultiAaGunWithNavalGunWithRadar = 10,

                [Description("摩耶改二 / 集中机枪 / 高角炮 / 无电探")]
                MayaGen2MultiAaGunWithNavalGunNoRadar = 11
            }

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
                clone.ZwCutInEquipts = (SimpleEquiptInfo[])ZwCutInEquipts.Clone();
                return clone;
            }
        }

        public class TorpedoInfo : ICloneable
        {
            internal int ZwFrom;
            internal int ZwTo;
            internal double ZwDamage;

            public ShipInfo From { get { return ZwFrom > 6 ? _parent.EnemyShips[ZwFrom - 7] : _parent.OurShips[ZwFrom - 1]; } }
            public ShipInfo To { get { return ZwTo > 6 ? _parent.EnemyShips[ZwTo - 7] : _parent.OurShips[ZwTo - 1]; } }
            //public int FromId { get { return ZwFrom; } }
            //public int ToId { get { return ZwTo; } }
            public double Damage { get { return ZwDamage; } }

            internal IShipInfoHolder _parent;
            public TorpedoInfo(IShipInfoHolder parent) { _parent = parent; ZwDamage = ZwFrom = ZwTo = 0; }

            object ICloneable.Clone()
            {
                return MemberwiseClone();
            }
        }

        public class BombardInfo : ICloneable
        {
            internal int ZwFrom;
            internal int[] ZwTo;
            internal AttackType ZwType;
            internal SimpleEquiptInfo[] ZwEquipts;
            internal double[] ZwDamage;

            public ShipInfo From { get { return ZwFrom > 6 ? _parent.EnemyShips[ZwFrom - 7] : _parent.OurShips[ZwFrom - 1]; } }
            public IEnumerable<ShipInfo> To { get { return ZwTo.Select(x => x > 6 ? _parent.EnemyShips[x - 7] : _parent.OurShips[x - 1]); } }
            //public int FromId { get { return ZwFrom; } }
            //public IReadOnlyList<int> ToId { get { return ZwTo; } }
            public AttackType Type { get { return ZwType; } }
            public IReadOnlyList<SimpleEquiptInfo> Equipts { get { return ZwEquipts; } }
            public IReadOnlyList<double> Damage { get { return ZwDamage; } }
            public IEnumerable<KeyValuePair<ShipInfo, double>> AttackInfos { get { return Enumerable.Zip(To, Damage, (a, b) => new KeyValuePair<ShipInfo, double>(a, b)); } }

            internal IShipInfoHolder _parent;
            public BombardInfo(IShipInfoHolder parent) { _parent = parent; }

            public enum AttackType
            {
                [Description("夜战CI - 三主炮")]
                ArtilleryCutIn = -5,

                [Description("夜战CI - 双主炮+副炮")]
                ArtilleryAndCannonCutIn = -4,

                [Description("夜战CI - 纯雷击")]
                TorpedoCutIn = -3,

                [Description("夜战CI - 炮雷混合")]
                ArtilleryAndTorpedoCutIn = -2,

                [Description("夜战二连")]
                NightTwiceInRow = -1,

                [Description("普通")]
                Normal = 0,

                //[Description("二连")]
                //TwiceInRow = 1,

                [Description("昼战二连")]
                DayTwiceInRow = 2,

                [Description("弹着观测 - 主炮+副炮")]
                ArtilleryWithCanonWithCorrection = 3,

                [Description("弹着观测 - 主炮+电探")]
                ArtilleryWithRadarCorrection = 4,

                [Description("弹着观测 - 主炮+彻甲弹")]
                ArtilleryWithApShellWithCorrection = 5,

                [Description("弹着观测 - 双主炮")]
                DualArtilleryWithCorrection = 6
            }

            object ICloneable.Clone()
            {
                var clone = (BombardInfo)MemberwiseClone();
                clone.ZwTo = (int[])ZwTo.Clone();
                clone.ZwEquipts = (SimpleEquiptInfo[])ZwEquipts.Clone();
                clone.ZwDamage = (double[])ZwDamage.Clone();
                return clone;
            }
        }

        public struct ShipHpStatus
        {
            public int Id { get; private set; }
            public int HpMax { get; private set; }
            public int HpCurrent { get; private set; }
            public string TypeName { get; private set; }
            public string ShipName { get; private set; }
            public Grabacr07.KanColleWrapper.Models.LimitedValue Hp { get { return new Grabacr07.KanColleWrapper.Models.LimitedValue(HpCurrent, HpMax, 0); } }
            private ShipInfo origInfo;

            public ShipHpStatus(ShipInfo info)
            {
                origInfo = info; HpMax = info.MaxHp; HpCurrent = info.CurrentHp; TypeName = info.ShipTypeName; ShipName = info.ShipName; Id = info.Id;
            }

            public ShipHpStatus ProcessBattle(BattleStatus report)
            {
                foreach(var aws3report in report.AirWarfare.EnemyStage3Report.SafeConcat(report.AirWarfare.OurStage3Report)) {
                    if(aws3report.Ship != origInfo) continue;
                    HpCurrent -= (int)aws3report.Damage;
                }
                foreach(var bmbreport in report.Bombards.SafeExpand(x => x).SafeConcat(report.NightWar?.Bombard)) {
                    foreach(var tgt in bmbreport.AttackInfos) {
                        if(tgt.Key != origInfo) continue;
                        HpCurrent -= (int)tgt.Value;
                    }
                }
                foreach(var tpreport in report.ClosingTorpedoAttack.SafeConcat(report.OpeningTorpedoAttack)) {
                    if(tpreport.To != origInfo) continue;
                    HpCurrent -= (int)tpreport.Damage;
                }

                if(HpCurrent < 0) HpCurrent = 0;

                return this;
            }
        }

        public struct SimpleEquiptInfo
        {
            private string ZwName;
            private int ZwId;

            public string Name { get { return ZwName; } }
            public int Id { get { return ZwId; } }

            public SimpleEquiptInfo(EquiptInfo ei)
            {
                ZwName = ei.EquiptName;
                ZwId = ei.EquiptId;
            }

            public SimpleEquiptInfo(SlotItemInfo info, int id = 0)
            {
                if(info != null) {
                    ZwId = info.Id;
                    ZwName = info.Name;
                } else {
                    ZwId = id;
                    ZwName = id.ToString();
                }
            }
        }

        public enum Formation
        {
            [Description("无")]
            None = 0,

            [Description("单纵")]
            SingleCol = 1,

            [Description("复纵")]
            DualCol = 2,

            [Description("轮型")]
            WheelType = 3,

            [Description("梯形")]
            Echelon = 4,

            [Description("单横")]
            SingleRow = 5
        }

        public enum EncounterForm
        {
            [Description("无")]
            None = 0,

            [Description("同航")]
            Cocurrent = 1,

            [Description("反航")]
            Inverse = 2,

            [Description("T有利")]
            FavourableT = 3,

            [Description("T不利")]
            UnfavourableT = 4,
        }

        public enum ReconnResult
        {
            [Description("无")]
            None = 0,

            [Description("敌舰队发现")]
            Success = 1,

            [Description("敌舰队发现 - 部分索敌机未返航")]
            SuccessWithLoss = 2,

            [Description("索敌机未返航")]
            ReconnLost = 3,

            [Description("未发现敌舰队")]
            Fail = 4,

            [Description("敌舰队发现 - 自主索敌成功")]
            AutonomousReconnSucceed = 5,

            [Description("无法进行索敌")]
            NoReconn = 6,
        }

        object ICloneable.Clone()
        {
            var clone = (BattleStatus)MemberwiseClone();
            clone.ZwEnemyShips = ZwEnemyShips.DeepCloneArray();
            clone.ZwOurShips = ZwOurShips.DeepCloneArray();
            clone.ZwAirWarfare = ZwAirWarfare.Clone();
            clone.ZwNightWar = ZwNightWar.Clone();
            clone.ZwOpeningTorpedoAttack = ZwOpeningTorpedoAttack.DeepCloneArray().ForEach(x => x._parent = clone);
            clone.ZwBombards = ZwBombards.DeepCloneArray().ForEach(x => x.ForEach(xx => xx._parent = clone));
            clone.ZwClosingTorpedoAttack = ZwClosingTorpedoAttack.DeepCloneArray().ForEach(x => x._parent = clone);
            return clone;
        }
    }
}
