using Grabacr07.KanColleWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LynLogger.DataStore.Serialization;

namespace LynLogger.DataStore.MasterInfo
{
    [Serializable]
    public class ShipNameType : AbstractDSSerializable<ShipNameType>
    {
        [Serialize(0)] public int ShipId { get; private set; }
        [Serialize(1)] public string ShipName { get; private set; }
        [Serialize(2)] public string TypeName { get; private set; }

        public ShipNameType(Premitives.StoragePremitive info, LinkedList<object> serializationPath) : base(info, serializationPath) { }

        public ShipNameType(int id)
        {
            var ship = KanColleClient.Current.Master.Ships[id];
            ShipId = id;
            ShipName = ship?.Name ?? ("Ship" + id);
            TypeName = ship?.ShipType.Name ?? ("Type"+id);
            if(ship != null) {
                ShipName = ship.Name;
                TypeName = ship.ShipType.Name;
                if(ship.RawData.api_yomi == "flagship" || ship.RawData.api_yomi == "elite") {
                    ShipName += ship.RawData.api_yomi;
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
