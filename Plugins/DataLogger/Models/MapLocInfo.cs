using LynLogger.DataStore.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LynLogger.DataStore.Premitives;

namespace LynLogger.Models
{
    public struct MapLocInfo : IDSSerializable
    {
        public int MapAreaId { get; set; }
        public int MapSectId { get; set; }
        public int MapLocId { get; set; }

        public string NodeId => string.Format("{0}-{1}-{2}", MapAreaId, MapSectId, MapLocId);

        public StoragePremitive GetSerializationInfo(LinkedList<object> _path)
        {
            return new Compound() {
                [16] = (SignedInteger)MapAreaId,
                [17] = (SignedInteger)MapSectId,
                [18] = (SignedInteger)MapLocId,
            };
        }

        public MapLocInfo(StoragePremitive info, LinkedList<object> serializationPath)
        {
            var comp = (Compound)info;
            MapAreaId = (int)(SignedInteger)comp[16];
            MapSectId = (int)(SignedInteger)comp[17];
            MapLocId = (int)(SignedInteger)comp[18];
        }

        public override bool Equals(object obj)
        {
            if(obj is MapLocInfo) {
                var o = (MapLocInfo)obj;
                return o.MapAreaId == MapAreaId && o.MapSectId == MapSectId && o.MapLocId == MapLocId;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return MapAreaId << 16 ^ MapSectId << 8 ^ MapLocId;
        }
    }
}
