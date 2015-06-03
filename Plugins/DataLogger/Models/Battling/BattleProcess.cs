using Grabacr07.KanColleWrapper.Models;
using LynLogger.DataStore.MasterInfo;
using LynLogger.DataStore.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace LynLogger.Models.Battling
{
    public interface IShipInfoHolder
    {
        IReadOnlyList<BattleProcess.ShipInfo> EnemyShips { get; }
        IReadOnlyList<BattleProcess.ShipInfo> OurShips { get; }
    }

    public partial class BattleProcess : AbstractDSSerializable<BattleProcess>, ICloneable, IShipInfoHolder
    {
        /*[Serialize(0)]*/ internal ShipInfo[] ZwEnemyShips;
        /*[Serialize(1)]*/ internal ShipInfo[] ZwOurShips;
        [Serialize(2)] internal Formation ZwOurFormation;
        [Serialize(3)] internal Formation ZwEnemyFormation;
        [Serialize(4, DepthLimit = 3)] internal EncounterForm ZwEncounter;
        [Serialize(5, DepthLimit = 3)] internal AirWarfareInfo ZwAirWarfare;
        [Serialize(6, DepthLimit = 3)] internal SupportInfo.Type ZwSupportType;
        [Serialize(7, DepthLimit = 3)] internal SupportInfo ZwSupport;
        [Serialize(8, DepthLimit = 3)] internal ReconnResult ZwOurReconn;
        [Serialize(9, DepthLimit = 3)] internal ReconnResult ZwEnemyReconn;
        /*[Serialize(10)]*/ internal TorpedoInfo[] ZwOpeningTorpedoAttack;
        /*[Serialize(11)]*/ internal BombardInfo[] ZwBombardRound1;
        /*[Serialize(12)]*/ internal BombardInfo[] ZwBombardRound2;
        /*[Serialize(13)]*/ internal BombardInfo[] ZwBombardRound3;
        /*[Serialize(14)]*/ internal TorpedoInfo[] ZwClosingTorpedoAttack;
        /*[Serialize(15)]*/ internal ShipHpStatus[] ZwOurShipBattleEndHp;
        /*[Serialize(16)]*/ internal ShipHpStatus[] ZwEnemyShipBattleEndHp;
        [Serialize(17, DepthLimit = 3)] internal bool ZwHasNightWar;
        [Serialize(18, DepthLimit = 3)] internal NightWarInfo ZwNightWar;
        [Serialize(19, DepthLimit = 3)] internal string ZwRawData;

        public IReadOnlyList<ShipInfo> EnemyShips => ZwEnemyShips;
        public IReadOnlyList<ShipInfo> OurShips => ZwOurShips;
        public Formation OurFormation => ZwOurFormation;
        public Formation EnemyFormation => ZwEnemyFormation;
        public EncounterForm Encounter => ZwEncounter;
        public AirWarfareInfo AirWarfare => ZwAirWarfare;
        public SupportInfo.Type SupportType => ZwSupportType;
        public SupportInfo Support => ZwSupport;
        public ReconnResult OurReconn => ZwOurReconn;
        public ReconnResult EnemyReconn => ZwEnemyReconn;
        public IReadOnlyList<TorpedoInfo> OpeningTorpedoAttack => ZwOpeningTorpedoAttack ?? (ZwOpeningTorpedoAttack = new TorpedoInfo[0]);
        public IReadOnlyList<TorpedoInfo> ClosingTorpedoAttack => ZwClosingTorpedoAttack ?? (ZwClosingTorpedoAttack = new TorpedoInfo[0]);
        public IReadOnlyList<ShipHpStatus> OurShipBattleEndHp => ZwOurShipBattleEndHp ?? (ZwOurShipBattleEndHp = Helpers.Sequence().Take(OurShips.Count).Select(x => new ShipHpStatus(x+1, this)).ToArray());
        public IReadOnlyList<ShipHpStatus> EnemyShipBattleEndHp => ZwEnemyShipBattleEndHp ?? (ZwEnemyShipBattleEndHp = Helpers.Sequence().Take(EnemyShips.Count).Select(x => new ShipHpStatus(x+7, this)).ToArray());
        public LimitedValue OurGuage => new LimitedValue(EnemyShipBattleEndHp.Sum(x => x.OrigInfo.CurrentHp - x.HpCurrent), EnemyShips.Sum(x => x.CurrentHp), 0);
        public LimitedValue EnemyGuage => new LimitedValue(OurShipBattleEndHp.Sum(x => x.OrigInfo.CurrentHp - x.HpCurrent), OurShips.Sum(x => x.CurrentHp), 0);
        public int OurGuagePerMil { get { var guage = OurGuage; return guage.Current * 1000 / guage.Maximum; } }
        public int EnemyGuagePerMil { get { var guage = EnemyGuage; return guage.Current * 1000 / guage.Maximum; } }
        public IReadOnlyList<BombardInfo> BombardRound1 => ZwBombardRound1;
        public IReadOnlyList<BombardInfo> BombardRound2 => ZwBombardRound2;
        public string RawData => ZwRawData;

        public bool HasNightWar => ZwHasNightWar;
        public NightWarInfo NightWar
        {
            get { return ZwNightWar; }
            set
            {
                if(ZwNightWar == null && HasNightWar) {
                    ZwNightWar = value.Clone();
                    ZwNightWar._parent = this;
                    ZwNightWar.ZwOurShips = null;
                    ZwNightWar.ZwEnemyShips = null;
                    ZwOurShipBattleEndHp = null;
                    ZwEnemyShipBattleEndHp = null;
                } else {
                    throw new InvalidOperationException();
                }
            }
        }
        
        public Ranking Rank
        {
            get
            {
                var our = OurShipBattleEndHp.ToList();
                var enemy = EnemyShipBattleEndHp.ToList();
                var ourPm = OurGuagePerMil;
                var enemyPm = EnemyGuagePerMil;
                var enemySunk = enemy.Count(x => x.HpCurrent == 0);
                var ourSunk = our.Count(x => x.HpCurrent == 0);

                if(ourSunk == 0) {
                    if(enemySunk == enemy.Count) return Ranking.S;

                    if(enemy.Count < 6 && enemySunk >= (enemy.Count+1)/2) return Ranking.A;
                    if(enemy.Count == 6 && enemySunk >= 4) return Ranking.A;

                    if(enemy[0].HpCurrent == 0) return Ranking.B;
                    if(ourPm > enemyPm * 2.5) return Ranking.B;

                    if(ourPm > enemyPm) return Ranking.C;
                } else {
                    if(enemy[0].HpCurrent == 0) {
                        if(ourSunk < enemySunk) return Ranking.B;
                        return Ranking.C;
                    }
                    if(ourPm > enemyPm * 2.5) {
                        if(enemy.Count < 6 && enemySunk >= (enemy.Count+1)/2) return Ranking.B;
                        if(enemy.Count == 6 && enemySunk == 4) return Ranking.B;
                    }
                    if(ourPm > enemyPm) {
                        return Ranking.C;
                    }
                    if(ourSunk >= (1+our.Count)/2) return Ranking.E;
                }

                return Ranking.D;
            }
        }

        public int DrillBasicExp
        {
            get
            {
                var bi = Data.LevelExperienceTable.Accumulated[EnemyShips[0].Lv] / 100;
                if (EnemyShips.Count > 1) {
                    bi += Data.LevelExperienceTable.Accumulated[EnemyShips[1].Lv] / 300;
                }
                if (bi > 500) {
                    bi = 500 + (int)Math.Sqrt(bi - 500);
                }
                return bi;
            }
        }

        public partial class NightWarInfo : AbstractDSSerializable<NightWarInfo>, ICloneable, IShipInfoHolder
        {
            /*[Serialize(0)]*/ internal ShipInfo[] ZwEnemyShips;
            /*[Serialize(1)]*/ internal ShipInfo[] ZwOurShips;
            /*[Serialize(2)]*/ internal BombardInfo[] ZwBombard;
            [Serialize(3)] internal string ZwOurReconnInTouchName;
            [Serialize(4)] internal int ZwOurReconnInTouch;
            [Serialize(5)] internal string ZwEnemyReconnInTouchName;
            [Serialize(6)] internal int ZwEnemyReconnInTouch;
            [Serialize(7, DepthLimit = 4)] internal string ZwRawData;

            internal IShipInfoHolder _parent;

            public IReadOnlyList<ShipInfo> EnemyShips => _parent?.EnemyShips ?? ZwEnemyShips;
            public IReadOnlyList<ShipInfo> OurShips => _parent?.OurShips ?? ZwOurShips;
            public IReadOnlyList<BombardInfo> Bombard => ZwBombard ?? (ZwBombard = new BombardInfo[0]);
            public int OurReconnInTouch => ZwOurReconnInTouch;
            public string OurReconnInTouchName => ZwOurReconnInTouchName;
            public int EnemyReconnInTouch => ZwEnemyReconnInTouch;
            public string EnemyReconnInTouchName => ZwEnemyReconnInTouchName;
            public string RawData => ZwRawData;

            internal NightWarInfo(IShipInfoHolder p) { _parent = p; }
        }

        public partial class ShipInfo : AbstractDSSerializable<ShipInfo>, ICloneable
        {
            [Serialize(0)] internal int ZwId;
            [Serialize(1)] internal int ZwShipId;
            [Serialize(2)] internal string ZwShipTypeName;
            [Serialize(3)] internal string ZwShipName;
            [Serialize(4)] internal int ZwLv;
            [Serialize(5)] internal int ZwCurrentHp;
            [Serialize(6)] internal int ZwMaxHp;
            [Serialize(7)] internal ParameterInfo ZwParameter;
            [Serialize(8)] internal ParameterInfo ZwEnhancement;
            /*[Serialize(9)]*/ internal EquiptInfo[] ZwEquipts;

            public int Id => ZwId;
            public int ShipId => ZwShipId;
            public string ShipTypeName => ZwShipTypeName;
            public string ShipName => ZwShipName;
            public int Lv => ZwLv;
            public int CurrentHp => ZwCurrentHp;
            public int MaxHp => ZwMaxHp;
            public ParameterInfo Parameter => ZwParameter;
            public ParameterInfo Enhancement => ZwEnhancement;
            public IReadOnlyList<EquiptInfo> Equipts => ZwEquipts;
            public int ShipAsControl => Equipts.Sum(x => x.SlotAsControl);

            public partial class ParameterInfo : AbstractDSSerializable<ParameterInfo>, ICloneable
            {
                [Serialize(0)] internal int ZwPower;
                [Serialize(1)] internal int ZwTorpedo;
                [Serialize(2)] internal int ZwAntiAir;
                [Serialize(3)] internal int ZwDefense;

                public int Power => ZwPower;
                public int Torpedo => ZwTorpedo;
                public int AntiAir => ZwAntiAir;
                public int Defense => ZwDefense;
            }
        }

        public partial class AirWarfareInfo : AbstractDSSerializable<AirWarfareInfo>, ICloneable
        {
            [Serialize(0)] internal AirspaceControl ZwOurAirspaceControl;

            /*[Serialize(1)]*/ internal int[] ZwOurCarrierShip;
            [Serialize(2)] internal int ZwOurStage1Engaged;
            [Serialize(3)] internal int ZwOurStage1Lost;
            [Serialize(4)] internal int ZwOurReconnInTouch;
            [Serialize(5)] internal int ZwOurStage2Engaged;
            [Serialize(6)] internal int ZwOurStage2Lost;
            [Serialize(7)] internal string ZwOurReconnInTouchName;

            /*[Serialize(8)]*/ internal int[] ZwEnemyCarrierShip;
            [Serialize(9)] internal int ZwEnemyStage1Engaged;
            [Serialize(10)] internal int ZwEnemyStage1Lost;
            [Serialize(11)] internal int ZwEnemyReconnInTouch;
            [Serialize(12)] internal int ZwEnemyStage2Engaged;
            [Serialize(13)] internal int ZwEnemyStage2Lost;
            [Serialize(14)] internal string ZwEnemyReconnInTouchName;

            /*[Serialize(15)]*/ internal bool[] ZwOurShipBombed;
            /*[Serialize(16)]*/ internal bool[] ZwOurShipTorpedoed;
            /*[Serialize(17)]*/ internal double[] ZwOurShipDamages;

            /*[Serialize(18)]*/ internal bool[] ZwEnemyShipBombed;
            /*[Serialize(19)]*/ internal bool[] ZwEnemyShipTorpedoed;
            /*[Serialize(20)]*/ internal double[] ZwEnemyShipDamages;

            [Serialize(21)] internal int ZwCutInShipNo = -1;
            [Serialize(22)] internal AaCutInType ZwCutInType;
            /*[Serialize(23)]*/ internal EquiptInfo[] ZwCutInEquipts;
            /*[Serialize(24)]*/ internal Stage3Report[] ZwOurStage3Report;
            /*[Serialize(25)]*/ internal Stage3Report[] ZwEnemyStage3Report;

            public AirspaceControl OurAirspaceControl => ZwOurAirspaceControl;
            public AirspaceControl EnemyAirspaceControl => (AirspaceControl)(6 - (int)ZwOurAirspaceControl);

            public IEnumerable<ShipInfo> OurCarrierShip => ZwOurCarrierShip.Select(x => _parent.OurShips[x]);
            public int OurStage1Engaged => ZwOurStage1Engaged;
            public int OurStage1Lost => ZwOurStage1Lost;
            public int OurReconnInTouch => ZwOurReconnInTouch;
            public int OurStage2Engaged => ZwOurStage2Engaged;
            public int OurStage2Lost => ZwOurStage2Lost;
            public string OurReconnInTouchName => ZwOurReconnInTouchName;

            public IEnumerable<ShipInfo> EnemyCarrierShip => ZwEnemyCarrierShip.Select(x => _parent.EnemyShips[x]);
            public int EnemyStage1Engaged => ZwEnemyStage1Engaged;
            public int EnemyStage1Lost => ZwEnemyStage1Lost;
            public int EnemyReconnInTouch => ZwEnemyReconnInTouch;
            public int EnemyStage2Engaged => ZwEnemyStage2Engaged;
            public int EnemyStage2Lost => ZwEnemyStage2Lost;
            public string EnemyReconnInTouchName => ZwEnemyReconnInTouchName;

            public ShipInfo CutInShip => (ZwCutInShipNo < 0 || ZwCutInShipNo > 5) ? null : _parent.OurShips[ZwCutInShipNo];
            public AaCutInType CutInType => ZwCutInType;
            public IReadOnlyList<EquiptInfo> CutInEquipts => ZwCutInEquipts;

            public int OurAsControlValue => OurCarrierShip.Sum(x => x.ShipAsControl);
            public int EnemyAsControlValue => EnemyCarrierShip.Sum(x => x.ShipAsControl);

            public IReadOnlyList<Stage3Report> OurStage3Report => ZwOurStage3Report ?? (ZwOurStage3Report = Helpers.Zip(ZwOurShipDamages, ZwOurShipBombed, ZwOurShipTorpedoed, Helpers.Sequence(),
                        (damage, bombed, torpedoed, ship) =>
                            new Stage3Report() {
                                ZwShip = ship+1,
                                ZwDamage = damage,
                                ZwBombed = bombed,
                                ZwTorpedoed = torpedoed,
                                _parent = _parent
                            }
                    ).Where(x => x.Bombed || x.Torpedoed).ToArray());

            public IReadOnlyList<Stage3Report> EnemyStage3Report => ZwEnemyStage3Report ?? (ZwEnemyStage3Report = Helpers.Zip(ZwEnemyShipDamages, ZwEnemyShipBombed, ZwEnemyShipTorpedoed, Helpers.Sequence(),
                        (damage, bombed, torpedoed, ship) =>
                            new Stage3Report() {
                                ZwShip = ship+7,
                                ZwDamage = damage,
                                ZwBombed = bombed,
                                ZwTorpedoed = torpedoed,
                                _parent = _parent
                            }
                    ).Where(x => x.Bombed || x.Torpedoed).ToArray());

            internal IShipInfoHolder _parent;
            public AirWarfareInfo(IShipInfoHolder parent) { _parent = parent; }

            public partial class Stage3Report : AbstractDSSerializable<Stage3Report>, ICloneable
            {
                [Serialize(0)] internal int ZwShip;
                [Serialize(1)] internal bool ZwBombed;
                [Serialize(2)] internal bool ZwTorpedoed;
                [Serialize(3)] internal double ZwDamage;

                internal IShipInfoHolder _parent;

                public ShipInfo Ship => ZwShip > 6 ? _parent.EnemyShips[ZwShip - 7] : _parent.OurShips[ZwShip - 1];
                public bool Bombed => ZwBombed;
                public bool Torpedoed => ZwTorpedoed;
                public double Damage => ZwDamage;
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
        }

        public partial class TorpedoInfo : AbstractDSSerializable<TorpedoInfo>, ICloneable
        {
            [Serialize(0)] internal int ZwFrom;
            [Serialize(1)] internal int ZwTo;
            [Serialize(2)] internal double ZwDamage;

            public ShipInfo From => ZwFrom > 6 ? _parent.EnemyShips[ZwFrom - 7] : _parent.OurShips[ZwFrom - 1];
            public ShipInfo To => ZwTo > 6 ? _parent.EnemyShips[ZwTo - 7] : _parent.OurShips[ZwTo - 1];
            public double Damage => ZwDamage;

            internal IShipInfoHolder _parent;
            public TorpedoInfo(IShipInfoHolder parent) { _parent = parent; ZwDamage = ZwFrom = ZwTo = 0; }
        }

        public partial class BombardInfo : AbstractDSSerializable<BombardInfo>, ICloneable
        {
            [Serialize(0)] internal int ZwFrom;
            /*[Serialize(1)]*/ internal int[] ZwTo;
            [Serialize(2)] internal AttackType ZwType;
            /*[Serialize(3)]*/ internal EquiptInfo[] ZwEquipts;
            /*[Serialize(4)]*/ internal double[] ZwDamage;

            public ShipInfo From => ZwFrom > 6 ? _parent.EnemyShips[ZwFrom - 7] : _parent.OurShips[ZwFrom - 1];
            public IEnumerable<ShipInfo> To => ZwTo.Select(x => x > 6 ? _parent.EnemyShips[x - 7] : _parent.OurShips[x - 1]);
            public AttackType Type => ZwType;
            public IReadOnlyList<EquiptInfo> Equipts => ZwEquipts;
            public IReadOnlyList<double> Damage => ZwDamage;
            public IEnumerable<KeyValuePair<ShipInfo, double>> AttackInfos => Enumerable.Zip(To, Damage, (a, b) => new KeyValuePair<ShipInfo, double>(a, b));

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
        }

        public partial class SupportInfo : AbstractDSSerializable<SupportInfo>, ICloneable
        {
            /*[Serialize(0)]*/ internal ShipInfo[] ZwSupportShips;
            [Serialize(1)] internal AirWarfareInfo ZwAttackInfo;

            public IReadOnlyList<ShipInfo> SupportShips => ZwSupportShips;
            public AirWarfareInfo AttackInfo => ZwAttackInfo;

            public enum Type
            {
                [Description("无")]
                None = 0,

                [Description("航空支援")]
                AirSupport = 1,

                [Description("炮击支援")]
                GunFight = 2,

                [Description("雷击支援")]
                Torpedo = 3
            }
        }

        public partial class ShipHpStatus : AbstractDSSerializable<ShipHpStatus>, ICloneable
        {
            [Serialize(0)] private FuzzyDouble ZwDeliveredDamage;
            [Serialize(1)] internal int ZwOrigShipId;
            [Serialize(2)] public int HpCurrent { get; private set; }

            internal BattleProcess _parent;

            public ShipInfo OrigInfo => ZwOrigShipId > 6 ? _parent.EnemyShips[ZwOrigShipId - 7] : _parent.OurShips[ZwOrigShipId - 1];
            public int Id => OrigInfo.Id;
            public string TypeName => OrigInfo.ShipTypeName;
            public string ShipName => OrigInfo.ShipName;
            public int HpMax => OrigInfo.MaxHp;

            public LimitedValue Hp => new LimitedValue(HpCurrent, HpMax, 0);
            public FuzzyDouble DeliveredDamage => ZwDeliveredDamage;

            public ShipHpStatus(int oid, BattleProcess parent)
            {
                _parent = parent; ZwOrigShipId = oid; HpCurrent = OrigInfo.CurrentHp; ZwDeliveredDamage = new FuzzyDouble();
                ProcessBattle(parent);
            }

            private ShipHpStatus ProcessBattle(BattleProcess report)
            {
                ShipInfo a = OrigInfo;

                if (report.AirWarfare.EnemyCarrierShip.Any(x => x == a)) {
                    if (report.AirWarfare.ZwEnemyCarrierShip.Length == 1) {
                        ZwDeliveredDamage += report.AirWarfare.ZwOurShipDamages.Sum();
                    } else {
                        ZwDeliveredDamage.UpperBound += report.AirWarfare.ZwOurShipDamages.Sum();
                    }
                } else if (report.AirWarfare.OurCarrierShip.Any(x => x == a)) {
                    if (report.AirWarfare.ZwOurCarrierShip.Length == 1) {
                        ZwDeliveredDamage += report.AirWarfare.ZwEnemyShipDamages.Sum();
                    } else {
                        ZwDeliveredDamage.UpperBound += report.AirWarfare.ZwEnemyShipDamages.Sum();
                    }
                }

                foreach(var aws3report in report.AirWarfare.EnemyStage3Report.SafeConcat(report.AirWarfare.OurStage3Report, report.Support?.AttackInfo.EnemyStage3Report)) {
                    if(aws3report.Ship != a) continue;
                    HpCurrent -= (int)aws3report.Damage;
                }
                foreach(var bmbreport in report.BombardRound1.SafeConcat(report.BombardRound2, report.NightWar?.Bombard)) {
                    foreach(var tgt in bmbreport.AttackInfos) {
                        if(tgt.Key != a) continue;
                        HpCurrent -= (int)tgt.Value;
                    }
                    if(bmbreport.From == a) {
                        ZwDeliveredDamage += bmbreport.AttackInfos.Sum(x => x.Value);
                    }
                }
                foreach(var tpreport in report.ClosingTorpedoAttack.SafeConcat(report.OpeningTorpedoAttack)) {
                    if (tpreport.To == a) {
                        HpCurrent -= (int)tpreport.Damage;
                    }
                    if(tpreport.From == a) {
                        ZwDeliveredDamage += tpreport.Damage;
                    }
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
            SingleRow = 5,

            [Description("第一警戒航行序列 (对潜警戒)")]
            JointFormation1 = 11,

            [Description("第二警戒航行序列 (前方警戒)")]
            JointFormation2 = 12,

            [Description("第三警戒航行序列 (防空警戒)")]
            JointFormation3 = 13,

            [Description("第四警戒航行序列 (通常战斗)")]
            JointFormation4 = 14,
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
    }

    public enum Ranking { S, A, B, C, D, E }
}
