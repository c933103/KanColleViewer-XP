using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.Models.Battling
{
    public class BattleStatus
    {
        internal ShipInfo[] ZwEnemyShips;
        public IReadOnlyList<ShipInfo> EnemyShips { get { return ZwEnemyShips; } }

        internal ShipInfo[] ZwOurShips;
        public IReadOnlyList<ShipInfo> OurShips { get { return ZwOurShips; } }

        internal Formation ZwOurFormation;
        public Formation OurFormation { get { return ZwOurFormation; } }

        internal Formation ZwEnemyFormation;
        public Formation EnemyFormation { get { return ZwEnemyFormation; } }

        internal EncounterForm ZwEncounter;
        public EncounterForm Encounter { get { return ZwEncounter; } }

        internal AirWarfareInfo ZwAirWarfare;
        public AirWarfareInfo AirWarfare { get { return ZwAirWarfare; } }

        internal TorpedoInfo[] ZwOpeningTorpedoAttack;
        public IReadOnlyList<TorpedoInfo> OpeningTorpedoAttack { get { return ZwOpeningTorpedoAttack ?? (ZwOpeningTorpedoAttack = new TorpedoInfo[0]); } }

        internal BombardInfo[][] ZwBombards;
        public IReadOnlyList<IReadOnlyList<BombardInfo>> Bombards { get { return ZwBombards ?? (ZwBombards = new BombardInfo[0][]); } }

        internal TorpedoInfo[] ZwClosingTorpedoAttack;
        public IReadOnlyList<TorpedoInfo> ClosingTorpedoAttack { get { return ZwClosingTorpedoAttack ?? (ZwClosingTorpedoAttack = new TorpedoInfo[0]); } }

        internal string ZwRawData;
        public string RawData { get { return ZwRawData; } }

        public IEnumerable<ShipHpStatus> OurShipBattleEndHp { get { return OurShips.Select(x => new ShipHpStatus(x)).Select(x => x.ProcessBattle(this)); } }

        public IEnumerable<ShipHpStatus> EnemyShipBattleEndHp { get { return EnemyShips.Select(x => new ShipHpStatus(x)).Select(x => x.ProcessBattle(this)); } }

        public class ShipInfo
        {
            internal int ZwId;
            public int Id { get { return ZwId; } }

            internal int ZwShipId;
            public int ShipId { get { return ZwShipId; } }

            internal string ZwShipTypeName;
            public string ShipTypeName { get { return ZwShipTypeName; } }

            internal string ZwShipName;
            public string ShipName { get { return ZwShipName; } }

            internal int ZwLv;
            public int Lv { get { return ZwLv; } }

            internal int ZwCurrentHp;
            public int CurrentHp { get { return ZwCurrentHp; } }

            internal int ZwMaxHp;
            public int MaxHp { get { return ZwMaxHp; } }

            internal ParameterInfo ZwParameter;
            public ParameterInfo Parameter { get { return ZwParameter; } }

            internal ParameterInfo ZwEnhancement;
            public ParameterInfo Enhancement { get { return ZwEnhancement; } }

            internal EquiptInfo[] ZwEquipts;
            public IReadOnlyList<EquiptInfo> Equipts { get { return ZwEquipts; } }

            public class ParameterInfo
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
        }

        public class AirWarfareInfo
        {
            internal AirspaceControl ZwOurAirspaceControl;

            internal int[] ZwOurCarrierShip;
            internal int ZwOurStage1Engaged;
            internal int ZwOurStage1Lost;
            internal int ZwOurReconnInTouch;
            internal int ZwOurStage2Engaged;
            internal int ZwOurStage2Lost;

            internal int[] ZwEnemyCarrierShip;
            internal int ZwEnemyStage1Engaged;
            internal int ZwEnemyStage1Lost;
            internal int ZwEnemyReconnInTouch;
            internal int ZwEnemyStage2Engaged;
            internal int ZwEnemyStage2Lost;

            internal bool[] ZwOurShipBombed;
            internal bool[] ZwOurShipTorpedoed;
            internal double[] ZwOurShipDamages;

            internal bool[] ZwEnemyShipBombed;
            internal bool[] ZwEnemyShipTorpedoed;
            internal double[] ZwEnemyShipDamages;

            public AirspaceControl OurAirspaceControl { get { return ZwOurAirspaceControl; } }
            public AirspaceControl EnemyAirspaceControl { get { return (AirspaceControl)(6 - (int)ZwOurAirspaceControl); } }

            public IEnumerable<ShipInfo> OurCarrierShip { get { return ZwOurCarrierShip.Select(x => _parent.OurShips[x]); } }
            public int OurStage1Engaged { get { return ZwOurStage1Engaged; } }
            public int OurStage1Lost { get { return ZwOurStage1Lost; } }
            public int OurReconnInTouch { get { return ZwOurReconnInTouch; } }
            public int OurStage2Engaged { get { return ZwOurStage2Engaged; } }
            public int OurStage2Lost { get { return ZwOurStage2Lost; } }

            public IEnumerable<ShipInfo> EnemyCarrierShip { get { return ZwEnemyCarrierShip.Select(x => _parent.EnemyShips[x]); } }
            public int EnemyStage1Engaged { get { return ZwEnemyStage1Engaged; } }
            public int EnemyStage1Lost { get { return ZwEnemyStage1Lost; } }
            public int EnemyReconnInTouch { get { return ZwEnemyReconnInTouch; } }
            public int EnemyStage2Engaged { get { return ZwEnemyStage2Engaged; } }
            public int EnemyStage2Lost { get { return ZwEnemyStage2Lost; } }

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

            private BattleStatus _parent;
            public AirWarfareInfo(BattleStatus parent) { _parent = parent; }

            public class Stage3Report
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
                [Description("未知 - 均衡或劣势")]
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

                [Description("未知 - 均衡或优势")]
                InvertNone = 6
            }
        }

        public class TorpedoInfo
        {
            internal int ZwFrom;
            internal int ZwTo;
            internal double ZwDamage;

            public ShipInfo From { get { return ZwFrom > 6 ? _parent.EnemyShips[ZwFrom - 7] : _parent.OurShips[ZwFrom - 1]; } }
            public ShipInfo To { get { return ZwTo > 6 ? _parent.EnemyShips[ZwTo - 7] : _parent.OurShips[ZwTo - 1]; } }
            public int FromId { get { return ZwFrom; } }
            public int ToId { get { return ZwTo; } }
            public double Damage { get { return ZwDamage; } }

            private BattleStatus _parent;
            public TorpedoInfo(BattleStatus parent) { _parent = parent; }
        }

        public class BombardInfo
        {
            internal int ZwFrom;
            internal int[] ZwTo;
            internal AttackType ZwType;
            internal EquiptInfo[] ZwEquipts;
            internal double[] ZwDamage;

            public ShipInfo From { get { return ZwFrom > 6 ? _parent.EnemyShips[ZwFrom - 7] : _parent.OurShips[ZwFrom - 1]; } }
            public IEnumerable<ShipInfo> To { get { return ZwTo.Select(x => x > 6 ? _parent.EnemyShips[x - 7] : _parent.OurShips[x - 1]); } }
            public int FromId { get { return ZwFrom; } }
            public IReadOnlyList<int> ToId { get { return ZwTo; } }
            public AttackType Type { get { return ZwType; } }
            public IReadOnlyList<EquiptInfo> Equipts { get { return ZwEquipts; } }
            public IReadOnlyList<double> Damage { get { return ZwDamage; } }
            public IEnumerable<KeyValuePair<ShipInfo, double>> AttackInfos { get { return Enumerable.Zip(To, Damage, (a, b) => new KeyValuePair<ShipInfo, double>(a, b)); } }

            private BattleStatus _parent;
            public BombardInfo(BattleStatus parent) { _parent = parent; }

            public enum AttackType
            {
                [Description("普通")]
                Normal = 0,

                [Description("二连")]
                TwiceInRow = 1,

                [Description("CI")]
                CutIn = 2,

                [Description("弹着观测")]
                WithCorrection = 3
            }
        }

        public class ShipHpStatus
        {
            public int Id { get; private set; }
            public int HpMax { get; private set; }
            public int HpCurrent { get; private set; }
            public string TypeName { get; private set; }
            public string ShipName { get; private set; }
            public Grabacr07.KanColleWrapper.Models.LimitedValue Hp { get { return new Grabacr07.KanColleWrapper.Models.LimitedValue(HpCurrent, HpMax, 0); } }
            private Models.Battling.BattleStatus.ShipInfo origInfo;

            public ShipHpStatus(Models.Battling.BattleStatus.ShipInfo info)
            {
                origInfo = info; HpMax = info.MaxHp; HpCurrent = info.CurrentHp; TypeName = info.ShipTypeName; ShipName = info.ShipName; Id = info.Id;
            }

            public ShipHpStatus ProcessBattle(Models.Battling.BattleStatus report)
            {
                foreach(var aws3report in report.AirWarfare.EnemyStage3Report.SafeConcat(report.AirWarfare.OurStage3Report)) {
                    if(aws3report.Ship != origInfo) continue;
                    HpCurrent -= (int)aws3report.Damage;
                }
                foreach(var bmbreport in report.Bombards.SafeExpand(x => x)) {
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
    }
}
