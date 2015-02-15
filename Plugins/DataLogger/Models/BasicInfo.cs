using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models.Raw;
using System.Xml.Serialization;

namespace LynLogger.Models
{
    [Serializable]
    public class BasicInfo
    {
        public int Level { get; internal set; }
        public int Experience { get; internal set; }
        public int FurnitureCoin { get; internal set; }

        public int Fuel { get; internal set; }
        public int Ammo { get; internal set; }
        public int Steel { get; internal set; }
        public int Bauxite { get; internal set; }
        public int HsBuild { get; internal set; }
        public int HsRepair { get; internal set; }
        public int DevMaterial { get; internal set; }
        public int ModMaterial { get; internal set; }

        public int ExerWins { get; internal set; }
        public int ExerLose { get; internal set; }
        public int OperWins { get; internal set; }
        public int OperLose { get; internal set; }
        public int ExpeWins { get; internal set; }
        public int ExpeLose { get; internal set; }

        public string Name { get; internal set; }

        public double Score
        {
            get
            {
                if(Level > 99) {
                    return (1.0 * OperWins + OperLose + (ExpeLose + ExpeWins) / 4.0) / (Level / 100.0 * (ExerLose + ExerWins));
                } else {
                    return (1.0 * OperWins + OperLose + (ExpeLose + ExpeWins) / 4.0) / (Math.Sqrt(Level) / 10 * (ExerLose + ExerWins));
                }
            }
        }

        private bool _dirty = false;

        internal BasicInfo()
        {
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
            DataStore.Instance.RaiseBasicInfoChange();
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
            DataStore.Instance.RaiseBasicInfoChange();
            _dirty = false;
        }
    }
}
