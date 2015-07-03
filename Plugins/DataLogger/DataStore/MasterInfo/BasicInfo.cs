using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LynLogger.DataStore.Serialization;
using Grabacr07.KanColleWrapper.Models.Raw;

namespace LynLogger.DataStore.MasterInfo
{
    public class BasicInfo : AbstractDSSerializable<BasicInfo>
    {
        [Serialize(0)] public int Level { get; private set; }
        [Serialize(1)] public int Experience { get; private set; }
        [Serialize(2)] public int FurnitureCoin { get; private set; }

        [Serialize(3)] public int Fuel { get; private set; }
        [Serialize(4)] public int Ammo { get; private set; }
        [Serialize(5)] public int Steel { get; private set; }
        [Serialize(6)] public int Bauxite { get; private set; }
        [Serialize(7)] public int HsBuild { get; private set; }
        [Serialize(8)] public int HsRepair { get; private set; }
        [Serialize(9)] public int DevMaterial { get; private set; }
        [Serialize(10)] public int ModMaterial { get; private set; }

        [Serialize(11)] public int ExerWins { get; private set; }
        [Serialize(12)] public int ExerLose { get; private set; }
        [Serialize(13)] public int OperWins { get; private set; }
        [Serialize(14)] public int OperLose { get; private set; }
        [Serialize(15)] public int ExpeWins { get; private set; }
        [Serialize(16)] public int ExpeLose { get; private set; }

        [Serialize(17)] public string Name { get; private set; }

        private bool _dirty = false;
        private Store _holder;

        internal BasicInfo(Store s)
        {
            _holder = s;
        }

        internal void Update(kcsapi_material[] ress, bool noUpdateEvent = false)
        {
            foreach(var res in ress) {
                switch(res.api_id) {
                    case 1:
                        if(Fuel == res.api_value) continue;
                        Fuel = res.api_value; _dirty = true;
                        break;
                    case 2:
                        if(Ammo == res.api_value) continue;
                        Ammo = res.api_value; _dirty = true;
                        break;
                    case 3:
                        if(Steel == res.api_value) continue;
                        Steel = res.api_value; _dirty = true;
                        break;
                    case 4:
                        if(Bauxite == res.api_value) continue;
                        Bauxite = res.api_value; _dirty = true;
                        break;
                    case 5:
                        if(HsBuild == res.api_value) continue;
                        HsBuild = res.api_value; _dirty = true;
                        break;
                    case 6:
                        if(HsRepair == res.api_value) continue;
                        HsRepair = res.api_value; _dirty = true;
                        break;
                    case 7:
                        if(DevMaterial == res.api_value) continue;
                        DevMaterial = res.api_value; _dirty = true;
                        break;
                    case 8:
                        if(ModMaterial == res.api_value) continue;
                        ModMaterial = res.api_value; _dirty = true;
                        break;
                }
            }
            if(!_dirty || noUpdateEvent) return;
            _holder.RaiseBasicInfoChange();
            _dirty = false;
        }

        internal void Update(kcsapi_basic data, bool noUpdateEvent = false)
        {
            if(!_dirty
                && (data.api_experience == Experience)
                && (data.api_fcoin == FurnitureCoin)
                && (data.api_level == Level)
                && (data.api_st_lose == OperLose)
                && (data.api_st_win == OperWins)
                && (data.api_pt_lose == ExerLose)
                && (data.api_pt_win == ExerWins)
                && (data.api_ms_count == ExpeWins+ExpeLose)
                && (data.api_ms_success == ExpeWins)
                && (data.api_nickname == Name))
                return;

            _dirty = true;
            Experience = data.api_experience;
            FurnitureCoin = data.api_fcoin;
            Level = data.api_level;
            OperWins = data.api_st_win;
            OperLose = data.api_st_lose;
            ExerWins = data.api_pt_win;
            ExerLose = data.api_pt_lose;
            ExpeWins = data.api_ms_success;
            ExpeLose = data.api_ms_count - data.api_ms_success;
            Name = data.api_nickname;

            if(!_dirty || noUpdateEvent) return;
            _holder.RaiseBasicInfoChange();
            _dirty = false;
        }

        public BasicInfo(Premitives.StoragePremitive info, LinkedList<object> serializationPath) : base(info, serializationPath) { _holder = (Store)serializationPath.First(x => x is Store); }
    }
}
