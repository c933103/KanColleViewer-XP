﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Internal;
using Grabacr07.KanColleWrapper.Models.Raw;

namespace Grabacr07.KanColleWrapper.Models
{
	/// <summary>
	/// 母港に所属している艦娘を表します。
	/// </summary>
	public class Ship : RawDataWrapper<kcsapi_ship2>, IIdentifiable
	{
		private readonly Homeport homeport;

		/// <summary>
		/// この艦娘を識別する ID を取得します。
		/// </summary>
		public int Id => this.RawData.api_id;

		/// <summary>
		/// 艦娘の種類に基づく情報を取得します。
		/// </summary>
		public ShipInfo Info { get; private set; }

		public int SortNumber => this.RawData.api_sortno;

		/// <summary>
		/// 艦娘の現在のレベルを取得します。
		/// </summary>
		public int Level => this.RawData.api_lv;

		/// <summary>
		/// 艦娘がロックされているかどうかを示す値を取得します。
		/// </summary>
		public bool IsLocked => this.RawData.api_locked == 1;

		/// <summary>
		/// 艦娘の現在の累積経験値を取得します。
		/// </summary>
		public int Exp => this.RawData.api_exp.Get(0) ?? 0;

		/// <summary>
		/// この艦娘が次のレベルに上がるために必要な経験値を取得します。
		/// </summary>
		public int ExpForNextLevel => this.RawData.api_exp.Get(1) ?? 0;
        private AaCutInType _antiAirCutIn;
        public AaCutInType AntiAirCutIn
        {
            get { return _antiAirCutIn; }
            set
            {
                if(_antiAirCutIn == value) return;
                _antiAirCutIn = value;
                RaisePropertyChanged();
            }
        }

        private SpecialAttackType _battleSpecialAttack;
        public SpecialAttackType BattleSpecialAttack
        {
            get { return _battleSpecialAttack; }
            set
            {
                if(_battleSpecialAttack == value) return;
                _battleSpecialAttack = value;
                RaisePropertyChanged();
            }
        }

        private NightBattleAttackType _nightSpecialAttack;
        public NightBattleAttackType NightSpecialAttack
        {
            get { return _nightSpecialAttack; }
            set
            {
                if(_nightSpecialAttack == value) return;
                _nightSpecialAttack = value;
                RaisePropertyChanged();
            }
        }


        #region HP 変更通知プロパティ

        private LimitedValue _HP;

		/// <summary>
		/// 耐久値を取得します。
		/// </summary>
		public LimitedValue HP
		{
			get { return this._HP; }
			private set
			{
				this._HP = value;
				this.RaisePropertyChanged();

				if (value.IsHeavilyDamage())
				{
					this.Situation |= ShipSituation.HeavilyDamaged;
				}
				else
				{
					this.Situation &= ~ShipSituation.HeavilyDamaged;
				}
			}
		}

		#endregion

		#region Fuel 変更通知プロパティ

		private LimitedValue _Fuel;

		/// <summary>
		/// 燃料を取得します。
		/// </summary>
		public LimitedValue Fuel
		{
			get { return this._Fuel; }
			private set
			{
				this._Fuel = value;
				this.RaisePropertyChanged();
			}
		}

		#endregion

		#region Bull 変更通知プロパティ

		private LimitedValue _Bull;

		public LimitedValue Bull
		{
			get { return this._Bull; }
			private set
			{
				this._Bull = value;
				this.RaisePropertyChanged();
			}
		}

		#endregion


		#region Firepower 変更通知プロパティ

		private ModernizableStatus _Firepower;

		/// <summary>
		/// 火力ステータス値を取得します。
		/// </summary>
		public ModernizableStatus Firepower
		{
			get { return this._Firepower; }
			private set
			{
				this._Firepower = value;
				this.RaisePropertyChanged();

			}
		}

		#endregion

		#region Torpedo 変更通知プロパティ

		private ModernizableStatus _Torpedo;

		/// <summary>
		/// 雷装ステータス値を取得します。
		/// </summary>
		public ModernizableStatus Torpedo
		{
			get { return this._Torpedo; }
			private set
			{
				this._Torpedo = value;
				this.RaisePropertyChanged();

			}
		}

		#endregion

		#region AA 変更通知プロパティ

		private ModernizableStatus _AA;

		/// <summary>
		/// 対空ステータス値を取得します。
		/// </summary>
		public ModernizableStatus AA
		{
			get { return this._AA; }
			private set
			{
				this._AA = value;
				this.RaisePropertyChanged();
			}

		}

		#endregion

		#region Armer 変更通知プロパティ

		private ModernizableStatus _Armer;

		/// <summary>
		/// 装甲ステータス値を取得します。
		/// </summary>
		public ModernizableStatus Armer
		{
			get { return this._Armer; }
			private set
			{
				this._Armer = value;
				this.RaisePropertyChanged();

			}
		}

		#endregion

		#region Luck 変更通知プロパティ

		private ModernizableStatus _Luck;

		/// <summary>
		/// 運のステータス値を取得します。
		/// </summary>
		public ModernizableStatus Luck
		{
			get { return this._Luck; }
			private set
			{
				this._Luck = value;
				this.RaisePropertyChanged();
			}
		}

		#endregion


		#region Slots 変更通知プロパティ

		private ShipSlot[] _Slots;

		public ShipSlot[] Slots
		{
			get { return this._Slots; }
			set
			{
				if (this._Slots != value)
				{
					this._Slots = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region EquippedSlots 変更通知プロパティ

		private ShipSlot[] _EquippedSlots;

		public ShipSlot[] EquippedSlots
		{
			get { return this._EquippedSlots; }
			set
			{
				if (this._EquippedSlots != value)
				{
					this._EquippedSlots = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion


		/// <summary>
		/// 装備によるボーナスを含めた索敵ステータス値を取得します。
		/// </summary>
		public int ViewRange => this.RawData.api_sakuteki.Get(0) ?? 0;

		/// <summary>
		/// 火力・雷装・対空・装甲のすべてのステータス値が最大値に達しているかどうかを示す値を取得します。
		/// </summary>
		public bool IsMaxModernized => this.Firepower.IsMax && this.Torpedo.IsMax && this.AA.IsMax && this.Armer.IsMax;

		/// <summary>
		/// 現在のコンディション値を取得します。
		/// </summary>
		public int Condition => this.RawData.api_cond;

		/// <summary>
		/// コンディションの種類を示す <see cref="ConditionType" /> 値を取得します。
		/// </summary>
		public ConditionType ConditionType => ConditionTypeHelper.ToConditionType(this.RawData.api_cond);

		/// <summary>
		/// この艦が出撃した海域を識別する整数値を取得します。
		/// </summary>
		public int SallyArea => this.RawData.api_sally_area;
        
		#region Status 変更通知プロパティ

		private ShipSituation situation;

		public ShipSituation Situation
		{
			get { return this.situation; }
			set
			{
				if (this.situation != value)
				{
					this.situation = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		internal Ship(Homeport parent, kcsapi_ship2 rawData)
			: base(rawData)
		{
			this.homeport = parent;
			this.Update(rawData);
		}

		internal void Update(kcsapi_ship2 rawData)
		{
			this.UpdateRawData(rawData);

			this.Info = KanColleClient.Current.Master.Ships[rawData.api_ship_id] ?? ShipInfo.Dummy;
			this.HP = new LimitedValue(this.RawData.api_nowhp, this.RawData.api_maxhp, 0);
			this.Fuel = new LimitedValue(this.RawData.api_fuel, this.Info.RawData.api_fuel_max, 0);
			this.Bull = new LimitedValue(this.RawData.api_bull, this.Info.RawData.api_bull_max, 0);

			if (this.RawData.api_kyouka.Length >= 5)
			{
				this.Firepower = new ModernizableStatus(this.Info.RawData.api_houg, this.RawData.api_kyouka[0]);
				this.Torpedo = new ModernizableStatus(this.Info.RawData.api_raig, this.RawData.api_kyouka[1]);
				this.AA = new ModernizableStatus(this.Info.RawData.api_tyku, this.RawData.api_kyouka[2]);
				this.Armer = new ModernizableStatus(this.Info.RawData.api_souk, this.RawData.api_kyouka[3]);
				this.Luck = new ModernizableStatus(this.Info.RawData.api_luck, this.RawData.api_kyouka[4]);
			}

            if (this.Slots?.Length == rawData.api_slotnum) {
                this.Slots.ForEach((obj, idx) => obj.Update(homeport.Itemyard.SlotItems[rawData.api_slot[idx]], this.Info.RawData.api_maxeq.Get(idx) ?? 0, this.RawData.api_onslot.Get(idx) ?? 0));
                RaisePropertyChanged(nameof(Slots));
            } else {
                this.Slots = this.RawData.api_slot
                    .Take(rawData.api_slotnum)
                    .Select(id => this.homeport.Itemyard.SlotItems[id])
                    .Select((t, i) => new ShipSlot(t, this.Info.RawData.api_maxeq.Get(i) ?? 0, this.RawData.api_onslot.Get(i) ?? 0)).ToArray();
            }
			this.EquippedSlots = this.Slots.Where(x => x.Equipped).ToArray();

			if (this.EquippedSlots.Any(x => x.Item.Info.Type == SlotItemType.応急修理要員))
			{
				this.Situation |= ShipSituation.DamageControlled;
			}
			else
			{
				this.Situation &= ~ShipSituation.DamageControlled;
			}

            // Count different types of equipments
            var primaryGuns = 0;
            var heavyPrimaryGuns = 0;
            var secondaryGuns = 0;
            var torpedoes = 0;
            var observationPlanes = 0;
            var radars = 0;
            var apShells = 0;
            var aaShells = 0;
            var highAngleGun = 0;
            var comboGunAndDirector = 0;
            var aaFireDirector = 0;
            foreach(var itemInfo in EquippedSlots.Select(x => x.Item.Info)) {
                switch(itemInfo.Type) {
                    case SlotItemType.水上偵察機:
                    case SlotItemType.水上爆撃機:
                        observationPlanes++;
                        break;
                    default:
                        switch(itemInfo.IconType) {
                            case SlotItemIconType.MainCanonHeavy:
                                heavyPrimaryGuns++;
                                primaryGuns++;
                                break;
                            case SlotItemIconType.MainCanonLight:
                            case SlotItemIconType.MainCanonMedium:
                                primaryGuns++;
                                break;
                            case SlotItemIconType.SecondaryCanon:
                                secondaryGuns++;
                                break;
                            case SlotItemIconType.Rader:
                                radars++;
                                break;
                            case SlotItemIconType.APShell:
                                apShells++;
                                break;
                            case SlotItemIconType.AntiAircraftFireDirector:
                                aaFireDirector++;
                                break;
                            case SlotItemIconType.AAShell:
                                aaShells++;
                                break;
                            case SlotItemIconType.Torpedo:
                                if(itemInfo.Id != 41) { //甲標的
                                    torpedoes++;
                                }
                                break;
                            case SlotItemIconType.HighAngleGun:
                                switch(itemInfo.Id) {
                                    case 10: //12.7cm連装高角砲
                                    case 66: //8cm高角砲
                                    case 71: //10cm連装高角砲(砲架)
                                        secondaryGuns++;
                                        break;
                                    case 130: //12.7cm高角砲＋高射装置
                                        secondaryGuns++;
                                        comboGunAndDirector++;
                                        break;
                                    case 3: //10cm連装高角砲
                                    case 48: //12.7cm単装高角砲
                                    case 91: //12.7cm連装高角砲(後期型)
                                        primaryGuns++;
                                        break;
                                    case 122: //10cm高角砲＋高射装置
                                        primaryGuns++;
                                        comboGunAndDirector++;
                                        break;
                                }
                                highAngleGun++;
                                break;
                        }
                        break;
                }
            }

            SpecialAttackType sat = SpecialAttackType.None;
            if(observationPlanes > 0) {
                if(primaryGuns == 2 && secondaryGuns == 0 && apShells == 1 && radars == 0) sat |= SpecialAttackType.DualArtilleryWithCorrection;
                if(primaryGuns == 1 && secondaryGuns == 1 && apShells == 1 && radars == 0) sat |= SpecialAttackType.ArtilleryWithApShellWithCorrection;
                if(primaryGuns == 1 && secondaryGuns == 1 && apShells == 0 && radars == 1) sat |= SpecialAttackType.ArtilleryWithRadarCorrection;
                if(primaryGuns >= 1 && secondaryGuns >= 1) sat |= SpecialAttackType.ArtilleryWithCanonWithCorrection;
                if(primaryGuns >= 2) sat |= SpecialAttackType.DualArtillery;
            }
            BattleSpecialAttack = sat;

            if(heavyPrimaryGuns == 1 && aaShells == 1 && aaFireDirector == 1 && radars == 1) AntiAirCutIn = AaCutInType.ArtilleryAaT3ShellWithRadar;
            else if(comboGunAndDirector >= 2 && radars >= 1) AntiAirCutIn = AaCutInType.DualNavalAndAaGunWithRadar;
            else if(heavyPrimaryGuns >= 1 && aaShells >= 1 && aaFireDirector >= 1) AntiAirCutIn = AaCutInType.ArtilleryAaT3ShellNoRadar;
            else if(highAngleGun >= 1 && aaFireDirector >= 1 && radars >= 1) AntiAirCutIn = AaCutInType.NavalGunWithAaGunWithRadar;
            else if(comboGunAndDirector >= 1 && radars >= 1) AntiAirCutIn = AaCutInType.NavalAndAaGunWithRadar;
            else if(highAngleGun >= 1 && aaFireDirector >= 1) AntiAirCutIn = AaCutInType.NavalGunWithAaGunNoRadar;
            else if(Info.Id == 330 || Info.Id == 421) { //秋月
                     if(highAngleGun >= 2 && radars >= 1) AntiAirCutIn = AaCutInType.AkizukiDualNavalGunWithRadar;
                else if(highAngleGun >= 1 && radars >= 1) AntiAirCutIn = AaCutInType.AkizukiNavalGunWithRadar;
                else if(highAngleGun >= 2               ) AntiAirCutIn = AaCutInType.AkizukiDualNavalGunNoRadar;
            } else if(Info.Id == 428) { //摩耶改二
                if((highAngleGun - comboGunAndDirector) >= 1 && EquippedSlots.Any(x => x.Item.Info.Id == 131)) {
                    AntiAirCutIn = radars > 0 ? AaCutInType.MayaGen2MultiAaGunWithNavalGunWithRadar : AaCutInType.MayaGen2MultiAaGunWithNavalGunNoRadar;
                }
            } else AntiAirCutIn = AaCutInType.None;

                 if(                                                torpedoes >= 2) NightSpecialAttack = NightBattleAttackType.TorpedoCutIn;
            else if(primaryGuns >= 3                                           ) NightSpecialAttack = NightBattleAttackType.TriArtilleryCutIn;
            else if(primaryGuns == 2 && secondaryGuns >= 1                  ) NightSpecialAttack = NightBattleAttackType.DualArtilleryWithCannonCutIn;
            else if(primaryGuns == 2 && secondaryGuns == 0 && torpedoes == 1) NightSpecialAttack = NightBattleAttackType.DualArtilleryWithTorpedoCutIn;
            else if(primaryGuns == 1 &&                          torpedoes == 1) NightSpecialAttack = NightBattleAttackType.ArtilleryWithTorpedoCutIn;
            else if(primaryGuns == 2 && secondaryGuns == 0 && torpedoes == 0) NightSpecialAttack = NightBattleAttackType.DualArtillery;
            else if(primaryGuns == 1 && secondaryGuns >= 1 && torpedoes == 0) NightSpecialAttack = NightBattleAttackType.ArtilleryWithCannon;
            else if(secondaryGuns >= 2 && (torpedoes == 0 || torpedoes == 1)) NightSpecialAttack = NightBattleAttackType.DualCannon;
            else NightSpecialAttack = NightBattleAttackType.None;
        }


		internal void Charge(int fuel, int bull, int[] onslot)
		{
			this.Fuel = this.Fuel.Update(fuel);
			this.Bull = this.Bull.Update(bull);
			for (var i = 0; i < this.Slots.Length; i++) this.Slots[i].Current = onslot.Get(i) ?? 0;
		}

		internal void Repair()
		{
			var max = this.HP.Maximum;
			this.HP = this.HP.Update(max);
            if(Condition < 40) {
                this.RawData.api_cond = 40;
                RaisePropertyChanged(nameof(Condition));
                RaisePropertyChanged(nameof(ConditionType));
            }
		}

		public override string ToString()
		{
			return $"ID = {this.Id}, Name = \"{this.Info.Name}\", ShipType = \"{this.Info.ShipType.Name}\", Level = {this.Level}";
		}
	}
}
