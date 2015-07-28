using Grabacr07.KanColleWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LynLogger.DataStore.Serialization;
using Grabacr07.KanColleWrapper.Models;
using LynLogger.Extensions.RawDataWrapper;

namespace LynLogger.DataStore.MasterInfo
{
    [Serializable]
    public class ShipNameType : AbstractDSSerializable<ShipNameType>
    {
        [Serialize(0)] public int ShipId { get; internal set; }
        [Serialize(1)] public string ShipName { get; internal set; }
        [Serialize(2)] public string TypeName { get; internal set; }

        public ShipNameType(Premitives.StoragePremitive info, LinkedList<object> serializationPath) : base(info, serializationPath) { }

        public ShipNameType(int id) : this(KanColleClient.Current.Master.Ships[id], id) { }

        public ShipNameType(ShipInfo ship, int id)
        {
            ShipId = id;
            ShipName = ship?.Name ?? ("Ship" + id);
            TypeName = ship?.ShipType.Name ?? ("Type" + id);
            if (ship != null) {
                ShipName = ship.Name;
                TypeName = ship.ShipType.Name;

                var yomi = ship.GetRawData()?.api_yomi;
                if(yomi == "flagship" || yomi == "elite") {
                    ShipName += yomi;
                }
            } else {
                ShipName = "Ship" + id;
                TypeName = "Type" + id;
            }
        }

        public override bool Equals(object obj)
        {
            if(obj == null) return false;
            if(!(obj is ShipNameType)) return false;
            var typed = (ShipNameType)obj;
            return typed.ShipId == ShipId && typed.ShipName == ShipName && typed.TypeName == TypeName;
        }

        public override int GetHashCode()
        {
            return ShipId ^ ShipName.GetHashCode() ^ TypeName.GetHashCode();
        }
    }
}
