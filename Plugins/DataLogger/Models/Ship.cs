using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models.Raw;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LynLogger.Models
{
    [Serializable]
    public class Ship
    {
        public string DisplayName
        {
            get
            {
                var info = Helpers.LookupShipNameInfo(ShipId);
                return string.Format("[{0}] {1} {2}", Id, info.TypeName, info.ShipName);
            }
        }

        public int Id { get; private set; }
        public int ShipId { get; private set; }

        public int Level { get; internal set; }
        public int Exp { get; internal set; }
        public int ExpNext { get; internal set; }
        public int Hp { get; internal set; }
        public int HpMax { get; internal set; }
        public int Range { get; internal set; }
        public int Fuel { get; internal set; }
        public int Ammo { get; internal set; }
        public int DockTime { get; internal set; }
        public int DockFuel { get; internal set; }
        public int DockSteel { get; internal set; }
        public int SRate { get; internal set; }
        public int Condition { get; internal set; }

        public int Power { get; internal set; }
        public int Torpedo { get; internal set; }
        public int AntiAir { get; internal set; }
        public int Defense { get; internal set; }
        public int Maneuver { get; internal set; }
        public int AntiSub { get; internal set; }
        public int Scout { get; internal set; }
        public int Luck { get; internal set; }

        public int MaxRawPower { get; internal set; }
        public int MaxRawTorpedo { get; internal set; }
        public int MaxRawAntiAir { get; internal set; }
        public int MaxRawDefense { get; internal set; }
        public int MaxRawManeuver { get; internal set; }
        public int MaxRawAntiSub { get; internal set; }
        public int MaxRawScout { get; internal set; }
        public int MaxRawLuck { get; internal set; }

        public int EnhancedPower { get; internal set; }
        public int EnhancedTorpedo { get; internal set; }
        public int EnhancedAntiAir { get; internal set; }
        public int EnhancedDefense { get; internal set; }
        public int EnhancedLuck { get; internal set; }

        public bool Locked { get; internal set; }

        [OptionalField]
        internal EquiptInfo[] ZwEquipts;
        public IReadOnlyList<EquiptInfo> Equipts { get { return ZwEquipts ?? (ZwEquipts = new EquiptInfo[0]); } }

        private bool _dirty = false;

        internal Ship(int id, int shipId)
        {
            Id = id; ShipId = shipId;
        }

        internal void Update(kcsapi_ship2 data, bool noUpdateEvent = false)
        {
            if(!HasDifference(data) && !_dirty) return;
            _dirty = true;

            Level = data.api_lv;
            Exp = data.api_exp[0];
            ExpNext = data.api_exp[1];
            Hp = data.api_nowhp;
            HpMax = data.api_maxhp;
            Range = data.api_leng;
            Fuel = data.api_fuel;
            Ammo = data.api_bull;
            DockTime = (int)(data.api_ndock_time / 1000);
            DockSteel = data.api_ndock_item[1];
            DockFuel = data.api_ndock_item[0];
            SRate = data.api_srate;
            Condition = data.api_cond;
            Power = data.api_karyoku[0];
            MaxRawPower = data.api_karyoku[1];
            Torpedo = data.api_raisou[0];
            MaxRawTorpedo = data.api_raisou[1];
            AntiAir = data.api_taiku[0];
            MaxRawAntiAir = data.api_taiku[1];
            Defense = data.api_soukou[0];
            MaxRawDefense = data.api_soukou[1];
            Maneuver = data.api_kaihi[0];
            MaxRawManeuver = data.api_kaihi[1];
            AntiSub = data.api_taisen[0];
            MaxRawAntiSub = data.api_taisen[1];
            Scout = data.api_sakuteki[0];
            MaxRawScout = data.api_sakuteki[1];
            Luck = data.api_lucky[0];
            MaxRawLuck = data.api_lucky[1];
            ShipId = data.api_ship_id;

            ZwEquipts = new EquiptInfo[data.api_slotnum];
            for(int i = 0; i < data.api_slotnum; i++) {
                var slotItem = KanColleClient.Current.Homeport.Itemyard.SlotItems[data.api_slot[i]];
                ZwEquipts[i] = new EquiptInfo(slotItem, data.api_onslot[i], data.api_slot[i]);
            }

            EnhancedPower = data.api_kyouka[0];
            EnhancedTorpedo = data.api_kyouka[1];
            EnhancedAntiAir = data.api_kyouka[2];
            EnhancedDefense = data.api_kyouka[3];
            EnhancedLuck = data.api_kyouka[4];

            Locked = data.api_locked != 0;

            if(noUpdateEvent) return;

            DataStore.Instance.RaiseShipDataChange(Id);
            _dirty = false;
        }

        private bool HasDifference(kcsapi_ship2 data)
        {
            if(Level != data.api_lv) return true;
            if(Exp != data.api_exp[0]) return true;
            if(ExpNext != data.api_exp[1]) return true;
            if(Hp != data.api_nowhp) return true;
            if(HpMax != data.api_maxhp) return true;
            if(Range != data.api_leng) return true;
            if(Fuel != data.api_fuel) return true;
            if(Ammo != data.api_bull) return true;
            if(DockTime != (int)(data.api_ndock_time / 1000)) return true;
            if(DockSteel != data.api_ndock_item[1]) return true;
            if(DockFuel != data.api_ndock_item[0]) return true;
            if(SRate != data.api_srate) return true;
            if(Condition != data.api_cond) return true;
            if(Power != data.api_karyoku[0]) return true;
            if(MaxRawPower != data.api_karyoku[1]) return true;
            if(Torpedo != data.api_raisou[0]) return true;
            if(MaxRawTorpedo != data.api_raisou[1]) return true;
            if(AntiAir != data.api_taiku[0]) return true;
            if(MaxRawAntiAir != data.api_taiku[1]) return true;
            if(Defense != data.api_soukou[0]) return true;
            if(MaxRawDefense != data.api_soukou[1]) return true;
            if(Maneuver != data.api_kaihi[0]) return true;
            if(MaxRawManeuver != data.api_kaihi[1]) return true;
            if(AntiSub != data.api_taisen[0]) return true;
            if(MaxRawAntiSub != data.api_taisen[1]) return true;
            if(Scout != data.api_sakuteki[0]) return true;
            if(MaxRawScout != data.api_sakuteki[1]) return true;
            if(Luck != data.api_lucky[0]) return true;
            if(MaxRawLuck != data.api_lucky[1]) return true;
            if(ShipId != data.api_ship_id) return true;
            
            if(ZwEquipts?.Length != data.api_slotnum) return true;
            for(int i = 0; i < data.api_slotnum; i++) {
                if(ZwEquipts[i].Id != data.api_slot[i]) return true;
                if(ZwEquipts[i].EquiptCount != data.api_onslot[i]) return true;
            }

            if(EnhancedPower != data.api_kyouka[0]) return true;
            if(EnhancedTorpedo != data.api_kyouka[1]) return true;
            if(EnhancedAntiAir != data.api_kyouka[2]) return true;
            if(EnhancedDefense != data.api_kyouka[3]) return true;
            if(EnhancedLuck != data.api_kyouka[4]) return true;

            return Locked ^ (data.api_locked != 0);
        }
    }

    [Serializable]
    public struct ShipNameType
    {
        public int ShipId;
        public string ShipName;
        public string TypeName;
    }
}
