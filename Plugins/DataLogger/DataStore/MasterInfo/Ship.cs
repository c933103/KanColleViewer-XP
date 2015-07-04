using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models.Raw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LynLogger.DataStore.Serialization;
using LynLogger.DataStore.Extensions;
using LynLogger.Extensions.RawDataWrapper;

namespace LynLogger.DataStore.MasterInfo
{
    [Serializable]
    public class Ship : AbstractDSSerializable<Ship>
    {
        public string DisplayName
        {
            get
            {
                return string.Format("[{0}] {1} {2}", Id, ShipInfo.TypeName, ShipInfo.ShipName);
            }
        }

        [Serialize( 0)] public int Id { get; private set; }
        [Serialize( 1)] public ShipNameType ShipInfo { get; private set; }
        [Serialize( 2)] public int Level { get; private set; }
        [Serialize( 3)] public int Exp { get; private set; }
        [Serialize( 4)] public int ExpNext { get; private set; }
        [Serialize( 5)] public int Hp { get; private set; }
        [Serialize( 6)] public int HpMax { get; private set; }
        [Serialize( 7)] public int Range { get; private set; }
        [Serialize( 8)] public int Fuel { get; private set; }
        [Serialize( 9)] public int Ammo { get; private set; }
        [Serialize(10)] public int DockTime { get; private set; }
        [Serialize(11)] public int DockFuel { get; private set; }
        [Serialize(12)] public int DockSteel { get; private set; }
        [Serialize(13)] public int SRate { get; private set; }
        [Serialize(14)] public int Condition { get; private set; }

        [Serialize(15)] public int Power { get; private set; }
        [Serialize(16)] public int Torpedo { get; private set; }
        [Serialize(17)] public int AntiAir { get; private set; }
        [Serialize(18)] public int Defense { get; private set; }
        [Serialize(19)] public int Maneuver { get; private set; }
        [Serialize(20)] public int AntiSub { get; private set; }
        [Serialize(21)] public int Scout { get; private set; }
        [Serialize(22)] public int Luck { get; private set; }

        [Serialize(23)] public int MaxRawPower { get; private set; }
        [Serialize(24)] public int MaxRawTorpedo { get; private set; }
        [Serialize(25)] public int MaxRawAntiAir { get; private set; }
        [Serialize(26)] public int MaxRawDefense { get; private set; }
        [Serialize(27)] public int MaxRawManeuver { get; private set; }
        [Serialize(28)] public int MaxRawAntiSub { get; private set; }
        [Serialize(29)] public int MaxRawScout { get; private set; }
        [Serialize(30)] public int MaxRawLuck { get; private set; }

        [Serialize(31)] public int EnhancedPower { get; private set; }
        [Serialize(32)] public int EnhancedTorpedo { get; private set; }
        [Serialize(33)] public int EnhancedAntiAir { get; private set; }
        [Serialize(34)] public int EnhancedDefense { get; private set; }
        [Serialize(35)] public int EnhancedLuck { get; private set; }
    
        [Serialize(36)] public bool Locked { get; private set; }
        
        /*Serialize37*/ internal EquiptInfo[] ZwEquipts;
        public IReadOnlyList<EquiptInfo> Equipts => ZwEquipts ?? (ZwEquipts = new EquiptInfo[0]);
        
        [Serialize(38)] public int MaxFuel { get; private set; }
        [Serialize(39)] public int MaxAmmo { get; private set; }

        [NonSerialized] private bool _dirty = false;
        [NonSerialized] private Store _holder;

        public Ship(Premitives.StoragePremitive info, LinkedList<object> serializationPath) : base(info, serializationPath)
        {
            _holder = (Store)serializationPath.First(x => x is Store);
        }

        internal Ship(int id, Store s)
        {
            Id = id; //ShipInfo = new ShipNameType(shipId);
            _holder = s;
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
            ShipInfo = new ShipNameType(data.api_ship_id);

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

            var shipInfo = KanColleClient.Current.Master.Ships[ShipInfo.ShipId];
            MaxAmmo = shipInfo?.GetRawData().api_bull_max ?? Ammo;
            MaxFuel = shipInfo?.GetRawData().api_fuel_max ?? Fuel;

            if(noUpdateEvent) return;

            _holder.RaiseShipDataChange(Id);
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
            if(ShipInfo.ShipId != data.api_ship_id) return true;

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

        protected override IReadOnlyDictionary<ulong, HandlerInfo> CustomFieldHandlers
        {
            get
            {
                return new Dictionary<ulong, HandlerInfo>() {
                    [37] = new HandlerInfo(
                        (x, p) => x.ZwEquipts.GetSerializationInfo(p, (k, p1) => (Premitives.Compound)k.GetSerializationInfo(p1)),
                        (o, i, p) => o.ZwEquipts = ((Premitives.List<Premitives.Compound>)i).Convert(x => new EquiptInfo(x, p)).ToArray()),
                };
            }
        }
    }
}
